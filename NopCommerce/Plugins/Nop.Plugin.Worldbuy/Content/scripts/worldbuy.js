jQuery(document).ready(function () {
    if (jQuery(".worldbuy-checkbox-list").length > 0) {
        jQuery(".worldbuy-checkbox-list").each(function () {
            var $this = jQuery(this);
            if ($this.children(".worldbuy-collapse-button").length > 0) {
                var $collapseButton = $this.children(".worldbuy-collapse-button");
                var data_normal_title = $collapseButton.children("span").attr("data-normal-title");
                var data_collapse_title = $collapseButton.children("span").attr("data-collapse-title");
                if ($this.hasClass("worldbuy-collapse")) {
                    $collapseButton.children("span").text(data_collapse_title);
                }
                else {
                    $collapseButton.children("span").text(data_normal_title);
                }
                $collapseButton.click(function () {
                    var parent = $collapseButton.parents(".worldbuy-checkbox-list");
                    if (parent.hasClass("worldbuy-collapse")) {
                        parent.removeClass("worldbuy-collapse");
                        $collapseButton.children("span").text(data_normal_title);
                    }
                    else {
                        parent.addClass("worldbuy-collapse");
                        $collapseButton.children("span").text(data_collapse_title);
                    }
                });
            }
        });
    }

    if (jQuery(".worldbuy-category-collapse-button").length > 0) {
        jQuery(".worldbuy-category-collapse-button").each(function () {
            var $this = jQuery(this);
            $this.click(function () {
                var parent = $this.parent();
                if (parent.hasClass("open")) {
                    parent.removeClass("open");
                }
                else {
                    parent.addClass("open");
                }
            })
        })
    }

    if (jQuery(".scrollbar-inner").length > 0) {
        jQuery('.scrollbar-inner').scrollbar();
    }
});