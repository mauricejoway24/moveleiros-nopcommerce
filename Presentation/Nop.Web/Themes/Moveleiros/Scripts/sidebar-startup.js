(function () {
    window.addEventListener('load', function () {
        var menuSidebar = new Sidebar('#toggle-sidebar-menu', '#sidebar-menu', 'right');
        var filterSidebar = new Sidebar('#toggle-sidebar-filter', '#sidebar-filter', 'left');
    });
})();