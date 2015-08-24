(function () { 
 
    // jQuery library is required in this sample 
    // Fallback to loading jQuery from a CDN path if the local is unavailable 
    (window.jQuery || document.write('<script src="//ajax.aspnetcdn.com/ajax/jquery/jquery-1.10.0.min.js"><\/script>')); 	
    //register css and javascript
    document.write('<script src="https://testremoteprovisioning.azurewebsites.net/15/js/bootstrap.min.js"><\/script>');
    document.write('<script src="https://testremoteprovisioning.azurewebsites.net/15/js/masonry.pkgd.min.js"><\/script>');
    document.write('<script src="https://testremoteprovisioning.azurewebsites.net/15/js/jquery.fancybox.pack.js"><\/script>');
    document.write('<script src="https://testremoteprovisioning.azurewebsites.net/15/js/raphael.min.js"><\/script>');
    document.write('<script src="https://testremoteprovisioning.azurewebsites.net/15/js/main.js"><\/script>');
    //css
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/buttons/buttons.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/buttons/social-icons.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/font-awesome.min.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/bootstrap.min.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/settings.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/jquery.fancybox.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/animate.css">'); 
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/style.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/responsive.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/customizer/pages.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/customizer/elements-pages-customizer.css">');
    document.write('<link rel="stylesheet" href="https://testremoteprovisioning.azurewebsites.net/15/css/ie/ie.css">');
   
    // Create object that have the context information about the field that we want to change it's output render  
    var imageViewerContext = {}; 
    imageViewerContext.Templates = {}; 
 
    // Be careful when add the header for the template, because it's will break the default list view render 
    imageViewerContext.Templates.Header = '<section id="main" class="no-padding">\
                                        <header class="page-header">\
                                            <div class="container">\
                                                <h1 class="title"><#=ctx.ListTitle#></h1>\
                                            </div>	\
                                        </header>\
                                        <div class="container">\
                                            <div class="row">\
                                                <div class="content col-sm-12 col-md-12">\
                                                    <div class="row">'; 
    imageViewerContext.Templates.Footer =   '</div>\
                                                     <div class="clearfix"></div>\
                                                </div>\
	                                        </div>\
                                        </div>\
                                    </section>';; 
 
    // Add OnPostRender event handler to add accordion click events and style 
    imageViewerContext.OnPostRender = function(){console.log("this is the end");}; 
 
    // This line of code tell TemplateManager that we want to change all HTML for item row render 
    imageViewerContext.Templates.Item = function(ctx){
        return '<div class="images-box col-sm-3 col-md-3">\
		                    <a class="gallery-images" rel="fancybox" href="'+ctx.CurrentItem["FileRef"]+'">\
			                    <img ng-src="'+ctx.CurrentItem["FileRef"]+'" width="200"  alt="'+ctx.CurrentItem["Title"]+'">\
			                    <span class="bg-images"><i class="fa fa-search"></i></span>\
		                    </a>\
	                    </div>';
    }; 
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(imageViewerContext); 
 
})(); 