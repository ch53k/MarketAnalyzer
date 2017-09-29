/// <binding ProjectOpened='Watch' />
// ReSharper disable UndeclaredGlobalVariableUsing
"use strict";

var vinylPaths = require('vinyl-paths');
var fs = require('fs');
var path = require('path');
var del = require("del");
var merge = require('merge-stream');
var minimist = require('minimist');
var mainBowerFiles = require('main-bower-files');

var gulp = require('gulp');
var less = require('gulp-less');
var sourcemaps = require('gulp-sourcemaps');
var minifyCSS = require('gulp-clean-css');
var rename = require("gulp-rename");
var uglify = require('gulp-uglify');
var concat = require('gulp-concat');
var iife = require("gulp-iife");
var order = require("gulp-order");
var gulpif = require('gulp-if');
var inject = require('gulp-inject');
var replace = require('gulp-replace');
var ngAnnotate = require('gulp-ng-annotate');
var print = require('gulp-print');
var templateCache = require('gulp-angular-templatecache');
var using = require('gulp-using');
var decomment = require('gulp-decomment');
var mainNpmFiles = require('gulp-main-npm-files');
var gutil = require('gulp-util');

//#region Variable Setup
var buildConfig = {
    string: 'buildConfig',
    default: { buildConfig: "Debug" }
};

var appConfig = { appMinJsName: 'marketAnalyzer' };

var options = minimist(process.argv.slice(2), buildConfig);
var forceReleaseBuild = false;
console.log('BuildConfig:', options.buildConfig);
var debug = options.buildConfig === 'Debug' && !forceReleaseBuild;

var getModules = function (dir) {
    return fs.readdirSync(dir)
        .filter(function (file) {
            return fs.statSync(path.join(dir, file)).isDirectory();
        });
};
//#endregion

//#region Debug Printing
gulp.task('PrintBowerFiles', function () {
    console.log('unfiltered files:', mainBowerFiles());
});

gulp.task('PrintPartialsList', function () {
    gulp.src(['Modules/**/*.html']).pipe(print());
});

gulp.task('PrintJsList', function () {
    gulp.src(gulpif(!debug, ['lib/js/oa.lm.*.js', 'lib/partials/*.js'], ['Modules/**/*.js', '!Modules/**/js/libraries/*.js']), { read: false })
        .pipe(order([
            '**/App/js/app.module.js',
            '**/*.module.js',
            '**/*.config.js',
            '**/*.service.js',
            '**/*.filter.js',
            '**/*.component.js',
            '**/*.directive.js',
            '**/*.tpls.min.js'
        ])).pipe(print());
});
//#endregion

//#region Clean
gulp.task('DeployClean', ['DeployCleanCss', 'DeployCleanImages', 'DeployCleanLibraryHtml', 'DeployCleanJs', 'DeployCleanPartials']);

gulp.task('DeployCleanCss', function () {
    return gulp.src(['lib/css/*.css', 'lib/css/icons/*.*', 'lib/fonts/*.*'], { read: false })
        .pipe(vinylPaths(del));
});

gulp.task('DeployCleanImages', function () {
    return gulp.src(['lib/images/*.*'], { read: false })
        .pipe(vinylPaths(del));
});

gulp.task('DeployCleanLibraryHtml', function () {
    return gulp.src(['lib/html/*.*'], { read: false })
        .pipe(vinylPaths(del));
});

gulp.task('DeployCleanJs', function () {
    return gulp.src(['lib/js/*.js'], { read: false })
        .pipe(vinylPaths(del));
});

gulp.task('DeployCleanPartials', function () {
    return gulp.src(['lib/partials/**/*.*'], { read: false })
        .pipe(vinylPaths(del));
});
//#endregion

//#region CSS
gulp.task('DeployCss', ['DeployCleanCss'], function () {
    var bowerFiles = gulp.src(mainBowerFiles('**/*.css'))
        .pipe(gulpif(!debug, minifyCSS()))
        .pipe(gulp.dest('lib/css'));

    var bowerIconFiles = gulp.src(mainBowerFiles('**/icons/*.*'))
        .pipe(gulp.dest('lib/css/icons'));

    var bowerFontFiles = gulp.src(mainBowerFiles('**/fonts/*.*'))
        .pipe(gulp.dest('lib/fonts'));

    var cssLessFiles = gulp.src('Modules/**/css/*.less')
        .pipe(gulpif(debug, sourcemaps.init()))
        .pipe(less())
        .pipe(decomment.text({ trim: true }))
        .pipe(gulpif(debug, sourcemaps.write()))
        .pipe(gulpif(!debug, minifyCSS()))
        .pipe(concat(appConfig.appMinJsName + '.css'))
        .pipe(gulp.dest('lib/css'));

    return merge(bowerFiles, bowerIconFiles, bowerFontFiles, cssLessFiles);
});

gulp.task('DeployCssLessOnly', function () {
    var cssLessFiles = gulp.src('Modules/**/css/*.less')
        .pipe(sourcemaps.init())
        .pipe(less())
        .pipe(decomment.text({ trim: true }))
        .pipe(sourcemaps.write())
        .pipe(gulpif(!debug, minifyCSS()))
        .pipe(concat(appConfig.appMinJsName + '.css'))
        .pipe(gulp.dest('lib/css'));

    return merge(cssLessFiles);
});
//#endregion

//#region Images
gulp.task('DeployImages', ['DeployCleanImages'], function () {
    var imageFiles = gulp.src('Modules/**/images/**/*.*')
        .pipe(rename({ dirname: '' }))
        .pipe(gulp.dest('lib/images'));

    return merge(imageFiles);
});
//#endregion

//#region HTML Snippets
gulp.task('DeployLibraryHtml', ['DeployCleanLibraryHtml'], function () {
    var bowerHtmlFiles = gulp.src(mainBowerFiles('**/html/*.*'))
        .pipe(gulp.dest('lib/html'));

    return merge(bowerHtmlFiles);
});
//#endregion

//#region JS
gulp.task('DeployJs', ['DeployCleanJs'], function () {
    var bowerFiles = gulp.src(mainBowerFiles('**/*.js'))
        .pipe(gulp.dest('lib/js'));

    var globalizeFiles = gulp.src(['bower_components/cldrjs/dist/cldr.js', 'bower_components/cldrjs/dist/cldr/event.js', 'bower_components/cldrjs/dist/cldr/supplemental.js', 'bower_components/globalize/dist/globalize.js', 'bower_components/globalize/dist/globalize/message.js', 'bower_components/globalize/dist/globalize/number.js', 'bower_components/globalize/dist/globalize/currency.js', 'bower_components/globalize/dist/globalize/date.js'])
        .pipe(concat('globalize.min.js'))
        //.pipe(uglify().on('error', gutil.log))
        .pipe(gulp.dest('lib/js'));

    //var npmFilesNonMin = gulp.src(mainNpmFiles(['**/*.js', '!**/*.min.js']))
    //    .pipe(rename({ dirname: '', suffix: '.min' }))
    //    .pipe(uglify())
    //    .pipe(gulp.dest('lib/js'));

    var libraryFiles = gulp.src('Modules/**/js/libraries/*.js')
        .pipe(rename({ dirname: '', suffix: '.min' }))
        .pipe(uglify())
        .pipe(gulp.dest('lib/js'));

    var moduleFiles = gulp.src(['Modules/**/*.js', '!Modules/**/js/libraries/*.js'])
        .pipe(order([
            '**/App/js/app.module.js',
            '**/*.module.js',
            '**/*.config.js',
            '**/*.service.js',
            '**/*.component.js',
            '**/*.directive.js'
        ]))
        .pipe(ngAnnotate({ add: true }))
        .pipe(iife({ prependSemicolon: false }))
        .pipe(concat(appConfig.appMinJsName + '.min.js'))
        .pipe(uglify().on('error', gutil.log))
        .pipe(gulp.dest('lib/js'));

    return merge(bowerFiles, globalizeFiles, libraryFiles, moduleFiles);
});
//#endregion

//#region HTML
gulp.task('DeployHtml', ['DeployCss', 'DeployImages', 'DeployLibraryHtml', 'DeployJs'], function () {
    var sources = gulp.src(gulpif(!debug, ['lib/js/marketAnalyzer.*.js', 'lib/partials/*.js'], ['Modules/**/*.js', '!Modules/**/js/libraries/*.js']), { read: false })
        .pipe(order([
            '**/lib/**/*.js',
            '**/App/js/app.module.js',
            '**/*.module.js',
            '**/*.config.js',
            '**/*.service.js',
            '**/*.filter.js',
            '**/*.component.js',
            '**/*.directive.js',
            '**/*.tpls.min.js'
        ]));

    var setJavaScriptFiles = gulp.src('index.template.html')
        .pipe(inject(sources, { removeTags: true, relative: true }))
        .pipe(rename("index.html"))
        .pipe(gulp.dest('./'));

    var removeJavaScriptSourceMapping = gulp.src('lib/js/*.min.js')
        .pipe(replace('//# sourceMappingURL=', '//'))
        .pipe(gulp.dest('lib/js'));

    return merge(setJavaScriptFiles, removeJavaScriptSourceMapping);
});
//#endregion

//#region Watch
gulp.task('WatchLess', function () {
    gulp.watch('Modules/**/*.less', ['DeployCssLessOnly']);
});
//#endregion

gulp.task('Deploy', ['DeployHtml'], function () { });
gulp.task('Watch', ['DeployCssLessOnly', 'WatchLess'], function () { })