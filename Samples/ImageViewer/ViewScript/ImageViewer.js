(function () {
    var BaseUrl= "https://yoursite.azurewebsites.net/";
    // jQuery library is required in this sample 
    // Fallback to loading jQuery from a CDN path if the local is unavailable 
    (window.jQuery || document.write('<script src="//ajax.aspnetcdn.com/ajax/jquery/jquery-1.10.0.min.js"><\/script>'));
    //register css and javascript
    document.write('<script src="'+BaseUrl+'15/js/bootstrap.min.js"><\/script>');
    document.write('<script src="'+BaseUrl+'15/js/masonry.pkgd.min.js"><\/script>');
    document.write('<script src="'+BaseUrl+'15/js/jquery.fancybox.pack.js"><\/script>');
    document.write('<script src="'+BaseUrl+'15/js/raphael.min.js"><\/script>');
    document.write('<script src="'+BaseUrl+'15/js/main.js"><\/script>');
    //css
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/buttons/buttons.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/buttons/social-icons.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/font-awesome.min.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/bootstrap.min.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/settings.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/jquery.fancybox.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/animate.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/style.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/responsive.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/customizer/pages.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/customizer/elements-pages-customizer.css">');
    document.write('<link rel="stylesheet" href="'+BaseUrl+'15/css/ie/ie.css">');

    // Create object that have the context information about the field that we want to change it's output render  
    var imageViewerContext = {};
    imageViewerContext.Templates = {};
    // Be careful when add the header for the template, because it's will break the default list view render 
    imageViewerContext.Templates.Header = '<section id="main" class="no-padding">\
                                        <header class="page-header">\
                                            <div class="container">\
                                            </div>	\
                                        </header>\
                                        <div class="container">\
                                            <div class="row">\
                                                <div class="content col-sm-12 col-md-12">\
                                                    <div class="row">';
    imageViewerContext.Templates.Footer = '</div>\
                                                     <div class="clearfix"></div>\
                                                </div>\
	                                        </div>\
                                        </div>\
                                    </section>';

    // Add OnPostRender event handler to add accordion click events and style 
    imageViewerContext.OnPostRender = function () {  };

    // This line of code tell TemplateManager that we want to change all HTML for item row render 
    imageViewerContext.Templates.Item = function (ctx) {
        var imgUrl =BaseUrl+"15/img/folder.png";
        var index = ctx.CurrentItem["FileRef"].lastIndexOf("/");
        var name = ctx.CurrentItem["Title"] == "" ? ctx.CurrentItem["FileRef"].substring(index + 1) : ctx.CurrentItem["Title"];
        if (ctx.CurrentItem.FSObjType == 0)
            imgUrl = ctx.CurrentItem["FileRef"];
        return '<div class="images-box col-sm-3 col-md-3">\
		                    <a class="gallery-images" rel="fancybox" href="'+ ctx.CurrentItem["FileRef"] + '">\
			                    <img src="'+ imgUrl + '" width="200"  alt="' + name + '">\
			                    <span class="bg-images">' + ctx.CurrentItem["Title"] + '</span>\
		                    </a>'+ name+'\
	                    </div>';
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(imageViewerContext);

})();