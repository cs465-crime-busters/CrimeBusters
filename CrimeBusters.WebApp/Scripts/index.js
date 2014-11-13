var userCoords = [];
var hiMarkers = [];
var loMarkers = [];
var reports = [];
var search = false;

$(function () {
    var map = $.getMap();
    $("#searchPanel").tabs();
    $.plotUsersOnMap(map);
    $("input#searchByDateButton").button();

    $("input#fromDate").datepicker({
        altField: "#altFieldFrom",
        altFormat: "mm/dd/yy",
        showAnim: "slideDown"
    });

    $("input#toDate").datepicker({
        altField: "#altFieldTo",
        altFormat: "mm/dd/yy",
        showAnim: "slideDown"
    });

    $("#reportTypeSelection").buttonset();

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

	    if (confirm("Are you sure you want to sign out?")) {
	        e.preventDefault();
	        $.logOffUser();
	    }
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

    $(document).on("click", "a.ackReport", function(e) {
        e.preventDefault();

        $(this).text("Acknowledging Report...");

        var reportId = $(this).attr("data-reportId");
        $.acknowledgeReport(reportId);
    });

    $(document).on("click", "a.closeReport", function (e) {
        e.preventDefault();

        var reportId = $(this).attr("data-reportId");
        $.closeReport(reportId);
    });


    $(document).on("click", "a.reports", function(e) {
        e.preventDefault();

        $.zoomUser(map, $(this).attr("data-markerId"), $(this).attr("data-reportType"));

        var mediaUrl = $(this).attr("data-mediaUrl");
        $.showPreview(mediaUrl);
       
    });

    $("input#searchByDateButton").on("click", function(e) {
        e.preventDefault();

        var reportTypeId = $("input[name='CrimeTypeList']:checked").val();
        var fromDateString = $("input#altFieldFrom").val();
        var toDateString = $("input#altFieldTo").val();

        $.getReportsFromSearch(reportTypeId, fromDateString, toDateString);
    });
});

(function ($) {
    //get map for google maps
	$.getMap = function() {
		var mapOptions = {
				zoom: 15,
				mapTypeId: google.maps.MapTypeId.ROADMAP,
				center: new google.maps.LatLng(40.106287, -88.225483)
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
	        url: "../Services/Index.asmx/GetActiveReports",
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
	                    icon: this.ReportTypeId == 1 ? "../Content/images/hi.png" : "../Content/images/lo.png",
	                    animation: google.maps.Animation.BOUNCE
	                });
	                marker.markerId = this.ReportId; //marker.markerId = index;
	                reports[index].markerId = this.ReportId;

	                if (this.ReportTypeId == 1) {
	                    hiMarkers.unshift(marker);
	                } else {
	                    loMarkers.unshift(marker);
	                }

	                // Shows the local time in the browser.
	                // workaround for Safari
	                var s = (this.TimeStampString + "Z").split(/[^0-9]/);
	                var tst = new Date(s[2], s[0] - 1, s[1], s[3], s[4], s[5], 0); 
	                var offset = -((new Date()).getTimezoneOffset() / 60);
	                tst.setHours(tst.getHours() + offset);

	                var fullName = this.User.FirstName + " " + this.User.LastName;
	                var email = this.User.Email;
	                var phoneNumber = this.User.PhoneNumber;
	                var contactMethodPref = this.ContactMethodPref;

	                if (fullName == " ") {
	                    fullName = "anonymous";
	                    email = "anonymous";
	                    phoneNumber = "anonymous";
	                    contactMethodPref = "anonymous";
	                }

	                var content = "<div id='markerPopup'>" +
                                        "<h5>Report Details</h5>" +
                                        "<ul>" +
                                            "<li>Reported By: " + fullName + "</li>" +
                                            "<li>Email: " + email + "</li>" +
                                            "<li>Phone Number: " + phoneNumber + "</li>" +
                                            "<li>Preferred Contact Method: " + contactMethodPref + "</li>" +
                                            "<li>Report Type: " + this.ReportType + "</li>" +
                                            "<li>Crime Type: " + this.CrimeType + "</li>" +
                                            "<li>Message: " + this.Message + "</li>" +
                                            "<li>Date Reported: " + tst.toLocaleString() + "</li>" +
                                            "<li>GPS Coordinates: " + marker.getPosition().toString() + "</li>" +
                                            "<li>Location: " + this.Location + "</li>" +
                                        "</ul>" +
                                  "<div style='text-align: right'><a href='#' class='ackReport' data-reportId='" + this.ReportId + "'>Acknowledge Report</a> | " +
                                  "<a href='#' class='closeReport' data-reportId='" + this.ReportId + "'>Set Report as Inactive</a></div>" +
                                  "</div>";

	                $.attachInfo(map, content, marker);
	            });

	            $.showCrimesAndEmergencies(1);
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

    $.showCrimesAndEmergencies = function (pageNumber) {
        for (var i = (pageNumber - 1) * 100; i < pageNumber * 100; i++) {
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

            var location = subReport.Location;

            if (location == "") {
                location = "Unknown Location";
            }

            if (subReport.ReportTypeId == 1) {
                $("ul", "#emergencies").append("<li><a class='reports' href='#' data-reportType='EMERGENCY' data-markerId='" + subReport.markerId
                    + "' data-mediaUrl=''>Emergency on <em>" + location + "</em> at " + tst.toLocaleString() + "</a></li>");
            } else {
                $("ul", "#crimes").append("<li><a class='reports' href='#' data-reportType='CRIME' data-markerId='" + subReport.markerId
                    + "' data-mediaUrl='" + subReport.Media1 + "'>Crime on <em>" + location + "</em> at " + tst.toLocaleString() + "</a></li>");
            }
        }
    }

    $.getReportsFromSearch = function(reportTypeId, fromDateString, toDateString) {
        $.ajax({
            type: "POST",
            dataType: "json",
            data: JSON.stringify({ "reportTypeId": reportTypeId, "fromDate": fromDateString, "toDate": toDateString }),
            timeout: 10000,
            contentType: "application/json",
            url: "../Services/Index.asmx/GetReports",
            beforeSend: function() {
                $("ul", "#searchResultList").children().remove();
            },
            success: function(data) {
                $.each(data.d, function () {
                    var location = this.Location;
                    if (location == "") {
                        location = "Unknown Location";
                    }

                    var s = (this.TimeStampString + "Z").split(/[^0-9]/);
                    var tst = new Date(s[2], s[0] - 1, s[1], s[3], s[4], s[5], 0);
                    var offset = -((new Date()).getTimezoneOffset() / 60);
                    tst.setHours(tst.getHours() + offset);

                    var reportType = this.ReportTypeId == 1 ? "EMERGENCY" : "CRIME";

                    $("ul", "#searchResultList").append("<li><a class='reports' href='#' data-reportType='" 
                        + reportType + "' data-markerId='" + this.ReportId
                    + "' data-mediaUrl='" + this.Media1 + "'>" + reportType + " on <em>" + location + "</em> at " + tst.toLocaleString() + "</a></li>");
                });
            },
            error: function() {
                
            }
        });
    };

    $.showPreview = function (mediaUrl) {
        $("#previewPanel").children().remove();

        if ($.isImage(mediaUrl)) {
            $("#previewPanel").append(
                "<img src='" + mediaUrl.substr(2) + "' alt='Uploaded Image' height='400' width='300' />");
        } else if ($.isVideo(mediaUrl)) {
            $("#previewPanel").append(
                "<video width='320' height='240' controls>" +
                    "<source src='" + mediaUrl.substr(2) + "' type='video/mp4' />" +
                    "<source src='" + mediaUrl.substr(2) + "' type='video/ogg' />" +
                    "<source src='" + mediaUrl.substr(2) + "' type='video/webm' />" +
                    "Your browser does not support the video tag.</video>"
            );
        } else if ($.isAudio(mediaUrl)) {
            $("#previewPanel").append(
                "<audio controls>" +
                "<source src='" + mediaUrl.substr(2) + "' type='audio/mp3' />" +
                "<source src='" + mediaUrl.substr(2) + "' type='audio/ogg' />" +
                "<source src='" + mediaUrl.substr(2) + "' type='audio/mpeg' />" +
                "Your browser does not support the audio element." +
                "</audio>");
        }
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
                        "<source src='" + mediaUrl.substr(2) + "' type='video/mp4' />" +
                        "<source src='" + mediaUrl.substr(2) + "' type='video/ogg' />" +
                        "<source src='" + mediaUrl.substr(2) + "' type='video/webm' />" +
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
        if (reportType == "EMERGENCY") {
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

    $.isImage = function (imageUrl) {
        if (imageUrl.toLowerCase().indexOf(".gif") > 0 || 
            imageUrl.toLowerCase().indexOf(".png") > 0 ||
            imageUrl.toLowerCase().indexOf(".jpg") > 0 ||
            imageUrl.toLowerCase().indexOf(".jpeg") > 0) {
            return true;
        } else {
            return false;
        }
    }

    $.isVideo = function (videoUrl) {
        if (videoUrl.toLowerCase().indexOf(".mp4") > 0 ||
            videoUrl.toLowerCase().indexOf(".avi") > 0 ||
            videoUrl.toLowerCase().indexOf(".ogv") > 0 ||
            videoUrl.toLowerCase().indexOf(".webm") > 0) {
            return true;
        } else {
            return false;
        }
    }

    $.isAudio = function (audioUrl) {
        if (audioUrl.toLowerCase().indexOf(".mp3") > 0 ||
            audioUrl.toLowerCase().indexOf(".wav") > 0 ||
            audioUrl.toLowerCase().indexOf(".3gp") > 0) {
            return true;
        } else {
            return false;
        }
    }

    $.acknowledgeReport = function(reportId) {
        $.ajax({
            type: "POST",
            dataType: "json",
            data: JSON.stringify({ "reportId": reportId }),
            timeout: 10000,
            contentType: "application/json",
            url: "../Services/PushNotification.asmx/AcknowledgeReport",
            success: function(data) {
                if (data.d == "success") {
                    alert("Acknowledgement sent.");
                } else {
                    alert(data.d);
                }
            },
            error: function() {
                alert("Unable to communicate with the server. Please try again.");
            },
            complete: function() {
                $("a.ackReport").text("Acknowledge Report");
            }
        });
    };

    $.closeReport = function (reportId) {
        $.ajax({
            type: "POST",
            dataType: "json",
            data: JSON.stringify({ "reportId": reportId, "isActive": false }),
            timeout: 10000,
            contentType: "application/json",
            url: "../Services/Index.asmx/UpdateIsActive",
            success: function (data) {
                if (data.d == "success") {
                    alert("Report Deactivated.");
                } else {
                    alert(data.d);
                }
            },
            error: function () {
                alert("Unable to communicate with the server. Please try again.");
            }
        });
    }
})(jQuery);



