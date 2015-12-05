angular.module('shared').factory('reportUrlHelper', function () {
    var obj = {};

    function serialize(obj) {
        var str = [];
        for (var p in obj)
            if (obj.hasOwnProperty(p)) {
                str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
            }
        return str.join("&");
    }

    function toUrlByIdAndParams(reportId, reportParameters) {
        var parsedParameters = { reportId: reportId };
        reportParameters.forEach(function (param) {
            if(param.Value)
                parsedParameters[param.Key] = param.Value;
        });

        return serialize(parsedParameters);
    }

    obj.toUrlByIdAndParams = toUrlByIdAndParams;

    obj.toUrl = function (report) {
        return toUrlByIdAndParams(report.Id, report.Parameters);
    }

    return obj;
});