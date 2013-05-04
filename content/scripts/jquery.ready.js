/*********************************************************
 * jQuery.ready()... where is all starts!
 *********************************************************/

$(document).ready(function() {

    $('#topnav > ul').addClass('sf-menu').superfish();

    $('#search').fastLiveFilter('#search-data');
    
});
