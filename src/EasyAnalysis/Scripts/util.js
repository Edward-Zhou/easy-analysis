var Utility = (function () {
    function formatDate(originalDate) {
        var dd = originalDate.getDate();
        var mm = originalDate.getMonth() + 1;
        var yyyy = originalDate.getFullYear();

        if (dd < 10) {
            dd = '0' + dd
        }

        if (mm < 10) {
            mm = '0' + mm
        }

        return yyyy + '-' + mm + '-' + dd;
    }

    var date = new Date();

    //Use universal time instead of the local time
    var nowUtc = new Date(date.getTime() + (date.getTimezoneOffset() * 60000));

    var firstDayThisMonth = formatDate(new Date(nowUtc.getFullYear(), nowUtc.getMonth(), 1));
    var lastDayThisMonth = formatDate(new Date(nowUtc.getFullYear(), nowUtc.getMonth() + 1, 0));

    var firstDayLastMonth = formatDate(new Date(nowUtc.getFullYear(), nowUtc.getMonth() - 1, 1));
    var lastDayLastMonth = formatDate(new Date(nowUtc.getFullYear(), nowUtc.getMonth(), 0));

    var firstDayLastThreeMonth = formatDate(new Date(nowUtc.getFullYear(), nowUtc.getMonth() - 2, 1));

    return {
        getDateRange: function (name) {
            switch (name) {
                case 'tm': return [firstDayThisMonth, lastDayThisMonth];
                case 'lm': return [firstDayLastMonth, lastDayLastMonth];
                case 'l3m': return [firstDayLastThreeMonth, lastDayThisMonth];
                case 'l30d': return [formatDate(Date.today().add(-30).days()), formatDate(Date.today())];
                default: return null;
            }
        }
    }
})();