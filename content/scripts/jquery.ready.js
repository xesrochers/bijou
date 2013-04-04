/*********************************************************
 * jQuery.ready()... where is all starts!
 *********************************************************/

$(document).ready(function() {
    
    $('.ajax').click(function(e) {
        var href = $(this).attr('href');
        e.preventDefault();
        $('#contents').load(href);
    });

    /*$('.ajaxLoaded').live(function() {
        alert('hahaha');
        SyntaxHighlighter.all(); 
    });

    function ajaxLive() {
        $('.ajaxLoaded').live(function() {
            alert('hahaha');
            SyntaxHighlighter.all(); 
        });
    }*/
});
