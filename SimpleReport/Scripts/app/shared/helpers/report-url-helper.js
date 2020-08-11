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
            if (param.Value) {
                if (param.InputType === 5 && param.Value.startsWith(",")) { // multichoice and starts with ","
                    param.Value = param.Value.substr(1);
                }
                parsedParameters[param.Key] = param.Value;
            }
        });

        return serialize(parsedParameters);
    }

    function toCompleteUrl(reportId, reportParameters) {
        return "Home/ExecuteReport/?" + toUrlByIdAndParams(reportId, reportParameters);
    }

    obj.toUrlByIdAndParams = toUrlByIdAndParams;
    obj.toCompleteUrl = toCompleteUrl;
    obj.toUrl = function (report) {
        return toUrlByIdAndParams(report.Id, report.Parameters);
    }

    return obj;
});