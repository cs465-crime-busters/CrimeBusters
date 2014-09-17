var userCoords = [];
var hiMarkers = [];
var loMarkers = [];
var reports = [];

$(function () {
	var map = $.getMap();
	$.plotUsersOnMap(map);

    //shows the uploaded media sent from Android
	$(document).on("click", "a.viewUploadedMedia", function (e) {
	    e.preventDefault();
	    var mediaListUrl = $(this).nextAll("input[data-mediaUrl]:hidden");
	    $.showUploadedMedia(mediaListUrl);
	});

    //downloades file to client
    $(document).on("click", "a[data-fileUrl]", function(e) {
        e.preventDefault();

        window.location.href = "../Services/DownloadFile.ashx?file="
            + encodeURIComponent($(this).attr("data-fileUrl"));
    });

    //changes report types shown (ALL, HI, Lo)
	$("a[data-reporttype]", "ul.dropdown-menu").on("click", function(e) {
	    e.preventDefault();
	    var reportType = $(this).attr("data-reporttype");
        $.updateMapClientSide(map, reportType);
    });

    //signs out of application
	$("#signOut").on("click", function (e) {
	    e.preventDefault();
	    $.logOffUser();
	});

    //shows reports on dashboard once option picked
    $("a#showReports").on("click", function(e) {
        e.preventDefault();

        $.showReports(1);
    });

    //shows correct reports per page
    $(document).on("click", "#reportsDashboard .paging li a", function (e) {
        e.preventDefault();

        $.showReports(parseInt($(this).html()));
    });

    //functionality for choosing a report on dashboard that takes user to icon on map
    $(document).on("click", "#reportsDashboard tr", function () {
        var $tr = $(this);

        if ($tr.attr("data-hasCoordinates") == 0) {
            var buttons = {
                "Close": function() {
                    $(this).dialog("close");
                }
            };
            var mediaListUrl = $tr.find("td:last").find("input:hidden");

            if (mediaListUrl.length > 0) {
                buttons["View Uploaded Media"] = function () {
                    $.showUploadedMedia(mediaListUrl);
                }
            }

            $("<p>Cannot locate the report on the map without coordinates.</p>").dialog({
                title: "No Coordinates Found",
                modal: true,
                width: 500,
                buttons: buttons
            });

            return;
        }

        $("#reportsDashboard").on("dialogclose", function() {
            $.zoomUser(map, $tr.attr("data-markerId"), $tr.attr("data-reportType"));
        }).dialog("close");
    });
});

(function ($) {
    //get map for google maps
	$.getMap = function() {
		var mapOptions = {
				zoom: 15,
				mapTypeId: google.maps.MapTypeId.ROADMAP,
				center: new google.maps.LatLng(40.099876, -88.227857)
		};
			
		return new google.maps.Map($("#map").get(0), mapOptions);
	};
	
    //plot all reports on map
	$.plotUsersOnMap = function (map) {
	    var coords = [];
	    var location;

	    $.ajax({
	        type: "POST",
	        dataType: "json",
	        timeout: 10000,
	        contentType: "application/json",
	        url: "../Services/Index.asmx/GetReports",
	        success: function (data) {
	            reports = data.d;
	            $.each(data.d, function (index) {
	                // Used to workaround the issue when 2 markers are on the same position.
	                var lat = this.Latitude;
	                var lng = this.Longitude;
	                var hash = lat + lng;

	                hash = hash.replace(/\./g, "").replace(",", "").replace("-", "");

	                // check to see if we've seen this hash before
	                if (userCoords[hash] == null && coords[hash] == null) {
	                    location = new google.maps.LatLng(parseFloat(lat), parseFloat(lng));
	                    // store an indicator that we've seen this point before
	                    coords[hash] = 1;
	                } else {
	                    // add some randomness to this point
	                    var newLat = parseFloat(lat) + (Math.random() - .5) / 1500;
	                    var newLong = parseFloat(lng) + (Math.random() - .5) / 1500;

	                    // get the coordinate object
	                    location = new google.maps.LatLng(newLat.toFixed(6), newLong.toFixed(6));
	                }

	                var marker = new google.maps.Marker({
	                    position: location,
	                    map: map,
	                    title: this.User.UserName,
                        icon: this.MarkerImage,
	                    animation: google.maps.Animation.BOUNCE
	                });
	                marker.markerId = index;

	                if (this.ReportType == "HIGH") {
	                    hiMarkers.push(marker);
	                } else {
	                    loMarkers.push(marker);
	                }

	                // Shows the local time in the browser.
	                // workaround for Safari
	                var s = (this.TimeStampString + "Z").split(/[^0-9]/);
	                var tst = new Date(s[2], s[0] - 1, s[1], s[3], s[4], s[5], 0); 
	                var offset = -((new Date()).getTimezoneOffset() / 60);
	                tst.setHours(tst.getHours() + offset);

	                var content = "<div id='markerPopup'>" + this.User.FirstName + " " + this.User.LastName + " needs help!" +
                                        "<h3>User Details</h3>" +
                                        "<ul>" +
                                            "<li>Report Type: " + this.ReportType + "</li>" +
                                            "<li>Message: " + this.Message + "</li>" +
                                            "<li>Date Reported: " + tst.toLocaleString() + "</li>" +
                                            "<li>Gender: " + this.User.Gender + "</li>" +
                                            "<li>Email: " + this.User.Email + "</li>" +
                                            "<li>Phone Number: " + this.User.PhoneNumber + "</li>" +
                                            "<li>Address: " + this.User.Address + "</li>" +
                                            "<li>Zip Code: " + this.User.ZipCode + "</li>" +
                                            "<li>Current Location: " + marker.getPosition().toString() + "</li>" +
                                            "<li>Location: " + this.Location + "</li>" +
                                        "</ul>" +
                                  "</div>";

	                if (this.UrlList.length != 0) {
	                    content += "<a class='viewUploadedMedia' href='#'>View Uploaded Media</a>";

	                    for (var i in this.UrlList) {
	                        content += "<input type='hidden' data-mediaUrl='" + this.UrlList[i] + "' />";
	                    }
	                }
	                $.attachInfo(map, content, marker);
	            });
	        },
	        error: function (xhr, textStatus, errorThrown) {
	            alert("Error: " + errorThrown);
	        }
	    });
	};
	
	var infoWindow = new google.maps.InfoWindow({
	    maxWidth: 500,
	    disableAutoPan: false
	});
	$.attachInfo = function (map, content, marker) {
	    google.maps.event.addListener(marker, "click", function (e) {
	        infoWindow.setContent(content);
	        infoWindow.open(map, marker);
	        marker.setAnimation(null);
	    });
	};

    //show lo,hi, or all markers depending on chosen report type
    $.updateMapClientSide = function(map, reportType) {
        switch (reportType) {
            case "high":
                $.showMarkers(map, hiMarkers);
                $.clearMarkers(loMarkers);
                break;
            case "low":
                $.showMarkers(map, loMarkers);
                $.clearMarkers(hiMarkers);
                break;
            default:
                $.showMarkers(map, hiMarkers);
                $.showMarkers(map, loMarkers);
                break;
        }
    };

    //sets markers to show on the map
    $.showMarkers = function(map, markers) {
        for (var i in markers) {
            markers[i].setMap(map);
        }
    };

    //clears markers on the map
    $.clearMarkers = function(markers) {
        for (var i in markers) {
            markers[i].setMap(null);
        }
    };

    //log off user from application
    $.logOffUser = function () {
	    $.ajax({
	        type: "POST",
	        dataType: "json",
	        timeout: 10000,
	        contentType: "application/json",
	        url: "../Services/Login.asmx/LogOutUser",
	        success: function (data) {
	            window.location.href = 'Login.aspx';
	        },
	        error: function () {
	            alert("Unable to communicate with the server. Please try again.");
	        }
	    });
    };

    //show reports on dashboard given the page number
    $.showReports = function(pageNumber) {
        $("#reportsDashboard").children().remove();
        $("#reportsDashboard").append(
            "<table>" +
            "<thead><tr>" +
            "<th scope='col'>User</th>" +
            "<th scope='col'>Report Type</th>" +
            "<th scope='col'>Message</th>" +
            "<th scope='col'>Gender</th>" +
            "<th scope='col'>Email</th>" +
            "<th scope='col'>Phone Number</th>" +
            "<th scope='col'>Address</th>" +
            "<th scope='col'>Zip Code</th>" +
            "<th scope='col'>Coordinates</th>" +
            "<th scope='col'>Location</th>" +
            "<th scope='col'>Date Reported</th>" +
            "</tr></thead><tbody>");

        for (var i = (pageNumber - 1) * 10; i < pageNumber * 10; i++) {
            var subReport = reports[i];

            if (subReport == null) {
                break;
            }
            // Shows the local time in the browser.
            // workaround for Safari
            var s = (subReport.TimeStampString + "Z").split(/[^0-9]/);
            var tst = new Date(s[2], s[0] - 1, s[1], s[3], s[4], s[5], 0);
            var offset = -((new Date()).getTimezoneOffset() / 60);
            tst.setHours(tst.getHours() + offset);

            var noLocMediaUrls = "";
            if (isNaN(parseFloat(subReport.Latitude)) && subReport.UrlList.length != 0) {
                for (var j in subReport.UrlList) {
                    noLocMediaUrls += "<input type='hidden' data-mediaUrl='" + subReport.UrlList[j] + "' />";
                }
            }

            $("#reportsDashboard tbody").append(
                "<tr data-reportId='" + subReport.ReportId + "' data-reportType='" + subReport.ReportType +
                    "' data-markerId='" + i + "' data-hasCoordinates='" + (isNaN(parseFloat(subReport.Latitude)) ? 0 : 1) + "'><td>" +
                subReport.User.LastName + ", " + subReport.User.FirstName + "</td><td>" +
                subReport.ReportType + "</td><td>" +
                subReport.Message + "</td><td>" +
                subReport.User.Gender + "</td><td>" +
                subReport.User.Email + "</td><td>" +
                subReport.User.PhoneNumber + "</td><td>" +
                subReport.User.Address + "</td><td>" +
                subReport.User.ZipCode + "</td><td>" +
                subReport.Latitude + "," + subReport.Longitude + "</td><td>" +
                subReport.Location + "</td><td>" +
                tst.toLocaleString() + noLocMediaUrls + "</td></tr>");
        }

        $("#reportsDashboard").append("</tbody></table>");
        $.addPagination(reports.length, 10, "#reportsDashboard");

        $("#reportsDashboard").dialog({
            modal: true,
            title: "Reports",
            show: "blind",
            hide: "clip",
            width: 1200
        });
    };

    //show media thats been uploaded given the URL
    $.showUploadedMedia = function (mediaListUrl) {
        $("#uploadedMediaWindow").children().remove();
        $("#uploadedMediaWindow").append("<ul class='uploadedMedia'>");
        $.each(mediaListUrl, function () {
            var mediaUrl = $(this).attr("data-mediaUrl");
            if ($.isImage(mediaUrl)) {
                $("ul.uploadedMedia", "#uploadedMediaWindow").append(
                    "<li><img src='" + mediaUrl.substr(2) + "' alt='Uploaded Image' height='400' width='300' /></li>");
            } else if ($.isVideo(mediaUrl)) {
                $("ul.uploadedMedia", "#uploadedMediaWindow").append(
                    "<video width='320' height='240' controls>" +
                        "<source src='" + mediaUrl.substr(2) + "' type='video/mp4'>" +
                        "<source src='" + mediaUrl.substr(2) + "' type='video/ogg'>" +
                        "<source src='" + mediaUrl.substr(2) + "' type='video/webm'>" +
                        "Your browser does not support the video tag.</video>"
	            );
            } else if ($.isAudio(mediaUrl)) {
                $("ul.uploadedMedia", "#uploadedMediaWindow").append("<a data-fileUrl='~/" + mediaUrl.substr(2) + "' href='#' >Download Audio File</a>");
            }
        });

        $("#uploadedMediaWindow").append("</ul>");

        $("#uploadedMediaWindow").dialog({
            title: "Uploaded Media",
            show: "fade",
            hide: "clip",
            modal: true,
            width: "402px",
            minheight: "500px"
        });
    }

    //add pages to the report dashboard
    $.addPagination = function (total, maxRows, domToAppend) {
        var totalPage;
        if (parseInt(total) % parseInt(maxRows) != 0) {
            totalPage = parseInt(parseInt(total) / parseInt(maxRows)) + 1;
        } else {
            totalPage = parseInt(parseInt(total) / parseInt(maxRows));
        }

        if (totalPage > 1) { 
            $("<ul class='paging'></ul>").appendTo(domToAppend);
            for (var i = 0; i < totalPage; i++) {
                $("ul.paging", domToAppend).append("<li><a href='#' data-startRowIndex='" + (i * parseInt(maxRows)) + "'>" + (i + 1) + "</a></li>");
            }
        }
    };

    //zoom to map location of the icon of the report
    $.zoomMap = function(map, lat, lng) {
        var location = new google.maps.LatLng(lat, lng);
        map.setCenter(location);
        map.setZoom(4);
    };

    $.zoomUser = function (map, markerId, reportType) {
        if (reportType == "HIGH") {
            var userIndex = $.binarySearch(hiMarkers, markerId, 0, hiMarkers.length - 1);

            if (userIndex != -1) {
                var loc = hiMarkers[userIndex].getPosition();
                map.setCenter(loc);
                map.setZoom(18);

                google.maps.event.trigger(hiMarkers[userIndex], "click");
                return;
            }
        }

        var userIndex = $.binarySearch(loMarkers, markerId, 0, loMarkers.length - 1);
        if (userIndex != -1) {
            var location = loMarkers[userIndex].getPosition();
            map.setCenter(location);
            map.setZoom(18);

            google.maps.event.trigger(loMarkers[userIndex], 'click'); 
            return;
        }
    };

    $.binarySearch = function (markers, key, imin, imax) {
        if (imax < imin) {
            return -1;
        } else {
            var imid = $.midpoint(imin, imax);

            var 
            deleteThis = markers[imid].markerId;

            if (markers[imid].markerId > key) {
                return $.binarySearch(markers, key, imin, imid - 1);
            } else if (markers[imid].markerId < key) {
                return $.binarySearch(markers, key, imid + 1, imax);
            } else {
                return imid;
            }
        }
    };

    $.midpoint = function(imin, imax) {
        return imin + parseInt(parseInt(imax - imin) / 2);
    };

    $.isImage = function(imageUrl) {
        if (imageUrl.indexOf(".gif") > 0 || 
            imageUrl.indexOf(".png") > 0 ||
            imageUrl.indexOf(".jpg") > 0 ||
            imageUrl.indexOf(".jpeg") > 0) {
            return true;
        } else {
            return false;
        }
    }

    $.isVideo = function (videoUrl) {
        if (videoUrl.indexOf(".mp4") > 0 ||
            videoUrl.indexOf(".avi") > 0 ||
            videoUrl.indexOf(".ogv") > 0 ||
            videoUrl.indexOf(".webm") > 0) {
            return true;
        } else {
            return false;
        }
    }

    $.isAudio = function (audioUrl) {
        if (audioUrl.indexOf(".mp3") > 0 ||
            audioUrl.indexOf(".wav") > 0 ||
            audioUrl.indexOf(".3gp") > 0) {
            return true;
        } else {
            return false;
        }
    }
})(jQuery);



