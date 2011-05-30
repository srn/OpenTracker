// HTML5 placeholder plugin version 0.3
// Enables cross-browser* html5 placeholder for inputs, by first testing
// for a native implementation before building one.
//
// USAGE: 
//$('input[placeholder]').placeholder();

(function($){

  $.fn.placeholder = function(options) {
    return this.each(function() {
      if ( !("placeholder"  in document.createElement(this.tagName.toLowerCase()))) {
        var $this = $(this);
        var placeholder = $this.attr('placeholder');
        $this.val(placeholder).data('color', $this.css('color')).css('color', '#aaa');
        $this
          .focus(function(){ if ($.trim($this.val())===placeholder){ $this.val('').css('color', $this.data('color')); } })
          .blur(function(){ if (!$.trim($this.val())){ $this.val(placeholder).data('color', $this.css('color')).css('color', '#aaa'); } });
      }
    });
  };
})(jQuery);

// detect if browser supports transition, currently checks for webkit, moz, opera, ms
var cssTransitionsSupported = false;
(function() {
    var div = document.createElement('div');
    div.innerHTML = '<div style="-webkit-transition:color 1s linear;-moz-transition:color 1s linear;-o-transition:color 1s linear;-ms-transition:color 1s linear;-khtml-transition:color 1s linear;transition:color 1s linear;"></div>';
    cssTransitionsSupported = (div.firstChild.style.webkitTransition !== undefined) || (div.firstChild.style.MozTransition !== undefined) || (div.firstChild.style.OTransition !== undefined) || (div.firstChild.style.MsTransition !== undefined) || (div.firstChild.style.KhtmlTransition !== undefined) || (div.firstChild.style.Transition !== undefined);
    delete div;
})();

// perform JavaScript after the document is scriptable.
$(document).ready(function() {
    $(".tabs > ul").tabs("section > section");
    $(".accordion").tabs(".accordion > section", {tabs: 'header', effect: 'slide', initialIndex: 0});
    
    $('input[placeholder]').placeholder();

    $("input[type=date]").dateinput();

    $.fn.uniform && $("input:checkbox,input:radio,select,input:file").uniform();
    
    $('#wrapper > section > aside > nav > h2').click(function(e){
        $(this).toggleClass('collapsed').next().toggle(!$(this).hasClass('collapsed')); e.preventDefault();
    });
    
    $('#wrapper > section > section').scrollbar();

    // Animate sidebar if transitions is not supported
    !cssTransitionsSupported && $('#wrapper > section > aside > nav > ul li a').hover(function(){
        $(this).css('padding-right', '20px').stop().animate({paddingRight: 40});
    },function(){
        $(this).stop().animate({paddingRight: 20});
    });
    /**
     * Form Validators
     */
    // Regular Expression to test whether the value is valid
    $.tools.validator.fn("[type=time]", "Please supply a valid time", function (input, value) {
        return(/^\d\d:\d\d$/).test(value);
    });
    
    $.tools.validator.fn("[data-equals]", "Value not equal with the $1 field", function (input) {
        var name = input.attr("data-equals"), 
        field = this.getInputs().filter("[name=" + name + "]");
        return input.val() === field.val() ? true : [name];
    });
    
    $.tools.validator.fn("[minlength]", function (input, value) {
        var min = input.attr("minlength");
        
        return value.length >= min ? true : {
            en : "Please provide at least " + min + " character" + (min > 1 ? "s" : "") 
        };
    });
    
    $.tools.validator.localizeFn("[type=time]", {
        en : 'Please supply a valid time'
    });
    
    /**
     * setup the validators
     */
    $(".has-validation").validator({
        position : 'bottom left', 
        offset : [5, 0], 
        messageClass : 'form-error', 
        message : '<div><em/></div>'// em element is the arrow
    }).attr('novalidate', 'novalidate');

});

var doc, draggable;

$.fn.customdrag = function(conf) {

    // disable IE specialities
    //document.ondragstart = function () { return false; };

    conf = $.extend({x: true, y: true, drag: true}, conf);

    doc = doc || $(document).bind("mousedown mouseup", function(e) {

        var el = $(e.target);  

        // start 
        if (e.type == "mousedown" && el.data("drag")) {

            var offset = el.position(),
                 x0 = e.pageX - offset.left, 
                 y0 = e.pageY - offset.top,
                 start = true;    

            doc.bind("mousemove.drag", function(e) {  
                var x = e.pageX -x0, 
                     y = e.pageY -y0,
                     props = {};

                if (conf.x) { props.left = x; }
                if (conf.y) { props.top = y; } 

                if (start) {
                    el.trigger("dragStart");
                    start = false;
                }
                if (conf.drag) { el.css(props); }
                el.trigger("drag", [y, x]);
                draggable = el;
            }); 

            e.preventDefault();

        } else {

            try {
                if (draggable) {  
                    draggable.trigger("dragEnd");  
                }
            } finally { 
                doc.unbind("mousemove.drag");
                draggable = null; 
            }
        } 

    });

    return this.data("drag", true); 
};	

// Custom Vertical Scrollbar
// @author Bryan Briosos
// @license MIT, GPL2
(function($){
    $.fn.extend({
        scrollbar: function() {
            this.each(function(i) {
                $base = $(this);
                $base.wrapInner('<div class="viewport"/>').prepend('<div class="scrollbar-vertical"><div class="scrollbar-button-start"></div><div class="scrollbar-track-piece"><div class="scrollbar-thumb" style="top: 0"></div></div><div class="scrollbar-button-end"></div></div>');
                var $scrollbar = $('> .scrollbar-vertical', $base);
                $scrollbar[0].onselectstart = function() {return false;}
                var thumbheight = 0, trackheight = 0, barheight = 0, dragstart = false;
                
                var init = function(){
                    barheight = $scrollbar.height($base.height()).height();
                    trackheight = $('.scrollbar-track-piece', $scrollbar).height(barheight - ($('.scrollbar-button-start', $scrollbar).height() + $('.scrollbar-button-end', $scrollbar).height())).height();
                    thumbheight = $('.scrollbar-thumb', $scrollbar).height(Math.round(barheight * trackheight / $('> .viewport', $base)[0].scrollHeight)).height();
                    if (thumbheight >= trackheight) {
                        $('.scrollbar-thumb', $scrollbar).hide();
                    } else {
                        $('.scrollbar-thumb', $scrollbar).show();
                    }
                };
                init();
                
                setInterval(init, 1000);
                
                var updateDragTop = function(newpos) {
                    $('.scrollbar-thumb', $scrollbar).css('top', newpos + 'px');
                };

                $('> .viewport', $base).scroll(function(event) {
                    if (!dragstart) { // if the scroll thumb wasn't dragged
                        var newpos = Math.round($('> .viewport', $base).scrollTop() * $('.scrollbar-track-piece', $scrollbar).height() / $('> .viewport', $base)[0].scrollHeight);
                        if (newpos != parseInt($('.scrollbar-thumb', $scrollbar).css('top'))) {
                            updateDragTop(newpos);
                        }
                    }
                    
                    // fix date position
                    $("input.date").each(function(){
                        var api = $(this).data("dateinput");
                        if (api.isOpen()) {
                            api.hide().show();
                        }
                    });
                    // fix the validator position
                    $(".has-validation").each(function(){
                        $(this).data("validator").reflow();
                    });
                });
                
                $('.scrollbar-thumb', $scrollbar).customdrag({x: false}).bind("dragStart", function(){
                    dragstart = true;
                }).bind("drag", function(event, x, y) {
                    if (parseInt($(this).css('top')) < 0) {
                        $(this).css('top', '0px').trigger('dragEnd');
                        return false;
                    }
                    if (parseInt($(this).css('top')) > trackheight - thumbheight) {
                        $(this).css('top', (trackheight - thumbheight) + 'px').trigger('dragEnd');
                        return false;
                    }
                    $('> .viewport', $base).scrollTop($('> .viewport', $base)[0].scrollHeight / trackheight * parseInt($(this).css('top')));
                }).bind("dragEnd", function(){
                    $('> .viewport', $base).animate({scrollTop: $('> .viewport', $base)[0].scrollHeight / trackheight * parseInt($(this).css('top'))}, 100, 'linear', function(){
                        dragstart = false;
                    });
                }).parent().mousedown(function(e){
                    if (e.pageY > $('.scrollbar-thumb', $scrollbar).offset().top + thumbheight) {
                        $('> .viewport', $base).animate({scrollTop: '+='+$base.height()}, 'fast', 'swing');
                    }
                    if (e.pageY < $('.scrollbar-thumb', $scrollbar).offset().top) {
                        $('> .viewport', $base).animate({scrollTop: '-='+$base.height()}, 'fast', 'swing');
                    }
                });
            });
        }
    });
})(jQuery);