angular.module('shared').factory('reportParameterHelper', function () {
    var obj = {};

    function sortedParameters(parameters, sql) {
        if (!sql)
            return parameters;

        var order = sql.match(/(@[\w\d@]*)/g);//start with @, match any letter or digit, plus @. So '@test @t, @from_@to' returns [@test , @t , @from_@to].
        var sorted = [];
        var missing = [];

        for (var i = 0; i < parameters.length; i++) {
            sorted.push(undefined);
        }

        for (var j = 0; j < parameters.length; j++) {
            var index = order.indexOf(parameters[j].SqlKey);

            if (index !== -1) {
                sorted[index] = parameters[j];
            } else {
                missing.push(parameters[j]);
            }
        }

        sorted = _.compact(sorted);

        for (var k = 0; k < missing.length; k++) {
            sorted.push(missing[k]);
        }

       return sorted;
   }

   obj.sortedParameters = sortedParameters;

    return obj;
});