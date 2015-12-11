// ==UserScript==
// @name         MSDN IFrame Injection
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  try to take over the world!
// @author       You
// @match        https://social.msdn.microsoft.com/*
// @grant        none
// ==/UserScript==
/* jshint -W097 */
'use strict';

(function () {

    if (typeof window.addin !== 'undefined') {
        return;
    }

    'use strict';
    function UserException(message) {
        this.message = message;
        this.name = "UserException";
    }

    function OpenLinkNewTab(link) {
        window.open(link, '_blank');
    }

    try {
        var isAdded = document.getElementById('eas-iframe');

        if (isAdded !== null) {
            return;
        }

        var siteurl = document.URL;

        var regSite = "social..*.microsoft.com/Forums/.*/({{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}}{0,1})";

        var re = new RegExp(regSite);

        if (siteurl.match(re)) {

            var myArray = re.exec(siteurl);

            var sidebar = document.getElementById('sidebar');

            var firstChild = sidebar.getElementsByTagName('section')[0];

            var section = document.createElement('section');

            var url = 'https://analyzeit.azurewebsites.net/Redirection/Navigate/' + myArray[1] + '?external=mt&type=iframe'

            section.innerHTML = '<div id="eas-iframe" style="height:500px;"><iframe src="' + url + '" style="width:100%;height:100%;"></iframe></div>';

            sidebar.insertBefore(section, firstChild);
        }
    }
    catch (err) {
        //alert(err.message);
        if (confirm(err.message + ', report it now ?')) {
            OpenLinkNewTab("https://github.com/dream-365/easy-analysis/issues");
        } else {
            // Do nothing!
        }
    }

    window.addin = this;
})();