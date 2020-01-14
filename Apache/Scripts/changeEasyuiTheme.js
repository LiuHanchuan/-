function changeThemeFun(themeName) {/* 更换主题 */
    var link = $('html').find('link:first');
    link.attr('href', '/Content/themes/' + themeName + '/easyui.css');
    var $iframe = $('iframe');
    if ($iframe.length > 0) {
        for (var i = 0; i < $iframe.length; i++) {
            var ifr = $iframe[i];
            $(ifr).contents().find('link:first').attr('href', '/Content/themes/' + themeName + '/easyui.css');
        }

    }

    $.cookie('easyuiThemeName', themeName, {
        expires: 7,path: '/'
    });
};
if ($.cookie('easyuiThemeName')) {
    changeThemeFun($.cookie('easyuiThemeName'));
}