Function.registerNamespace('Insight.Common');
var defaultImage=_spPageContextInfo.siteAbsoluteUrl + '/Lists/VideoChannelStyle/Custom Style/img/defaultChannel.jpg';
Insight.Common.RenderVideoChannels = function (channels) {
    var html = "<div id=\"content\">";
    var template = '<div class="mosaic-block bar">\
			<a href="{0}" target="_blank" class="mosaic-overlay">\
				<div class="details">\
					<h4>{1}</h4>\
					<p>{2}</p>\
				</div>\
			</a>\
			<div class="mosaic-backdrop"><img src="{3}"/></div>\
		</div>';
    for (var i = 0; i < channels.length ; i++) {
        var channelItem = Insight.Common.GetChannelItem(channels[i]);
        var title = channelItem.Title;
        var desc = channelItem.ChannelDescription;
        var link = String.format(_spPageContextInfo.siteAbsoluteUrl + "/_layouts/15/videochannel.aspx?channel={0}", channelItem.ChannelID);
        var image = channelItem.ChannelImage == null ? defaultImage : channelItem.ChannelImage.Url;
        html += String.format(template, link, title, desc, image);
    }
    html += "</div>";
    $("[id$='channelGallery']").html(html);
    //execute mosaic script
    jQuery(function ($) {
        $('.bar').mosaic({
            animation: 'slide'		//fade or slide
        });
    });
}
Insight.Common.GetItems = function (listName, query, renderFunction) {
    SP.SOD.executeFunc('sp.js', 'SP.ClientContext', function () {
        //load the menu items list items with context social
        var context = SP.ClientContext.get_current();
        var web = context.get_web();
        var lists = web.get_lists();
        var olist = lists.getByTitle(listName);
        var camlQuery = new SP.CamlQuery();
        camlQuery.set_viewXml(query);
        var listItems = olist.getItems(camlQuery);
        context.load(listItems);
        context.executeQueryAsync(function () { renderFunction(listItems); }, onError);
    });

}
Insight.Common.GetItemsREST = function (listName, query, renderFunction) {
    var RESTUrl = _spPageContextInfo.siteAbsoluteUrl + "/_api/web/lists/GetByTitle('" + listName + "')/items?" + query;
    $.ajax(
                {
                    url: RESTUrl,
                    method: "GET",
                    headers: {
                        "accept": "application/json; odata=verbose",
                    },
                    success: function (data) {
                        if (data.d.results.length > 0) {
                            //render the data
                            renderFunction(data.d.results);
                        }
                        else {
                            renderFunction(null);
                        }
                    },
                    error: function (err) {
                    },
                }
            );
}
Insight.Common.GetVideoChannels = function (callback) {
    var RESTUrl = _spPageContextInfo.siteAbsoluteUrl + "/_api/VideoService/Channels";
    $.ajax(
                {
                    url: RESTUrl,
                    method: "GET",
                    headers: {
                        "accept": "application/json; odata=verbose",
                    },
                    success: function (data) {
                        if (data.d.results.length > 0) {
                            //render the data
                            callback(data.d.results);
                        }
                    },
                    error: function (err) {
                    },
                }
            );
}
Insight.Common.GetChannelItem = function (channel) {
    if (channelItems != null) {
        for (var i = 0; i < channelItems.length; i++) {
            if (channelItems[i].ChannelID == channel.Id)
                return channelItems[i];
        }
    }
    //if the item is not found 
    return JSON.parse('{"Title":"' + channel.Title + '","ChannelID":"' + channel.Id + '","ChannelDescription":"","ChannelImage":{"Url":"' +defaultImage+ '"}}');
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
var channelItems;
SP.SOD.executeFunc('sp.js', 'SP.ClientContext', function () {
    if (document.location.toString().indexOf("videochannels.aspx") > -1) {
        Insight.Common.LoadScript("https://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js", function () {
            Insight.Common.LoadCSS(_spPageContextInfo.siteAbsoluteUrl + "/Lists/VideoChannelStyle/Custom Style/css/mosaic.css", "screen", function () {
                Insight.Common.LoadScript(_spPageContextInfo.siteAbsoluteUrl + "/Lists/VideoChannelStyle/Custom Style/js/mosaic.1.0.1.min.js", function () {
                    //clear the channels html
                    $("[id$='channelGallery']").html("");
                    //get all channel items from the custom list
                    Insight.Common.GetItemsREST("VideoChannelsMetaData", '$select=Title,ChannelImage,ChannelID,ChannelDescription', function (items) {
                        //cache the channel items read from SP List
                        channelItems = items;
                        //load all channels via RESTful API
                        Insight.Common.GetVideoChannels(Insight.Common.RenderVideoChannels);
                    });
                });
            });
        }
        );
    }
});
