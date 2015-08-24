(function () {
    // Initialize the variable that stores the objects.
    var overrideCtx = {};
    overrideCtx.Templates = {};

    // Assign functions or plain html strings to the templateset objects:
    // header, footer and item.
    overrideCtx.Templates.Header = '<section id="main" class="no-padding">\
                                        <header class="page-header">\
                                            <div class="container">\
                                                <h1 class="title"><#=ctx.ListTitle#></h1>\
                                            </div>	\
                                        </header>\
                                        <div class="container">\
                                            <div class="row">\
                                                <div class="content col-sm-12 col-md-12">\
                                                    <div class="row">';
    // This template is assigned to the CustomItem function.
    overrideCtx.Templates.Item = customItem;
    overrideCtx.Templates.Footer =                  '</div>\
                                                     <div class="clearfix"></div>\
                                                </div>\
	                                        </div>\
                                        </div>\
                                    </section>';

    // Set the template to the:
    //  Picture Library definition ID 109
    //  Base view ID
    overrideCtx.BaseViewID = 2;
    overrideCtx.ListTemplateType = 109;

    // Assign a function to handle the
    // PreRender and PostRender events
    overrideCtx.OnPreRender = preRenderHandler;
    overrideCtx.OnPostRender = postRenderHandler;

    // Register the template overrides.
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
})();

// This function builds the output for the item template.
// It uses the context object to access announcement data.
function customItem(ctx) {

    // Build a listitem entry for every announcement in the list.
    var itemTemplate='<div class="images-box col-sm-3 col-md-3">\
		                    <a class="gallery-images" rel="fancybox" href="{0}">\
			                    <img ng-src="{0}" width="200"  alt="{1}">\
			                    <span class="bg-images"><i class="fa fa-search"></i></span>\
		                    </a>\
	                    </div>'
    return String.fromat(itemTemplate, ctx.CurrentItem.FileRef,ctx.CurrentItem.Title);
}

// The preRenderHandler attends the OnPreRender event
function preRenderHandler(ctx) {
    var hostUrl = _spPageContextInfo.siteAbsoluteUrl+"/Lists/Scripts";
    Insight.Common.LoadScript("https://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js", function () {
        Insight.Common.LoadCSS(hostUrl + "/15/css/mosaic.css", "screen", function () {
            Insight.Common.LoadCSS(hostUrl + "/15/css/jquery.fancybox.css", "screen", function () {
                Insight.Common.LoadCSS(hostUrl + "/15/css/style.css", "screen", function () {
                    Insight.Common.LoadScript("https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js", function () {
                        Insight.Common.LoadScript(hostUrl+"15/js/bootstrap.min.js", function () {
                            Insight.Common.LoadScript(hostUrl + "15/js/masonry.pkgd.min.js", function () {
                                Insight.Common.LoadScript(hostUrl + "15/js/jquery.fancybox.pack.js", function () {
                                    Insight.Common.LoadScript(hostUrl + "15/js/jquery.raphael.min.js", function () {
                                        Insight.Common.LoadScript(hostUrl + "15/js/main.js", function () {
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        });
    });     
}

// The postRenderHandler attends the OnPostRender event
function postRenderHandler(ctx) {

}


Insight.Common.LoadScript = function (src, callback) {
    var s,
        r,
        t;
    r = false;
    s = document.createElement('script');
    s.type = 'text/javascript';
    s.src = src;
    s.onload = s.onreadystatechange = function () {
        //console.log( this.readyState ); //uncomment this line to see which ready states are called.
        if (!r && (!this.readyState || this.readyState == 'complete')) {
            r = true;
            callback();
        }
    };
    document.getElementsByTagName('head')[0].appendChild(s);
}
Insight.Common.LoadCSS = function (src, media, callback) {
    var s,
        r,
        t;
    r = false;
    s = document.createElement('link');
    s.type = 'text/css';
    s.rel = "stylesheet";
    s.media = media;
    s.href = src;
    s.onload = s.onreadystatechange = function () {
        //console.log( this.readyState ); //uncomment this line to see which ready states are called.
        if (!r && (!this.readyState || this.readyState == 'complete')) {
            r = true;
            callback();
        }
    };
    document.getElementsByTagName('head')[0].appendChild(s);
}
