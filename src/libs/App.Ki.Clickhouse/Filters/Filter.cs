using System.Collections;
using System.Text;

namespace App.Ki.Clickhouse.Filters;

public enum LogicOperator
{
    NONE,
    AND,
    OR,
    IN
}

public class Filter
{
    public LogicOperator PrevOperator { get; set; }
    public LogicOperator NextOperator { get; set; }
    public string FieldName { get; set; }
    public object ParameterValue { get; set; }
    public string Operator { get; set; }

    public string ToCriteria()
    {
        var parameterValue = "";

        var type = ParameterValue.GetType();

        if (type == typeof(int))
            parameterValue += (int) ParameterValue;
        else if (type == typeof(Int64))
            parameterValue += (Int64) ParameterValue;
        else if (type == typeof(string))
            parameterValue = ParameterValue.Equals(")")
                             || string.IsNullOrEmpty(ParameterValue.ToString())
                             || ParameterValue.Equals("''")
                ? ParameterValue.ToString()
                : $"'{ParameterValue}'";
        else if (type == typeof(DateTime))
            parameterValue = ((DateTime) ParameterValue).ToString("'dd:MM:yyyy HH:mm:ss'");
        else if (type == typeof(bool))
            parameterValue = (bool) ParameterValue ? "1" : "0";
        else if (ParameterValue is IEnumerable enumerable)
        {
                
            var enumerator = enumerable.GetEnumerator();
            enumerator.Reset();
            var values = new StringBuilder();
            while (enumerator.MoveNext())
            {
                var value = enumerator.Current;
                values.Append(value is string ? $"'{value}'" : value?.ToString() ?? "null");
                values.Append(",");
            }

            Operator = "In";
            parameterValue = $"({values.ToString().Trim(',')})";
        }


        return
            $"{(PrevOperator != LogicOperator.NONE ? PrevOperator.Equals(LogicOperator.AND) ? "AND" : "OR" : "")} {FieldName} {Operator} {parameterValue} {(NextOperator != LogicOperator.NONE ? NextOperator.Equals(LogicOperator.AND) ? "AND" : "OR" : "")}";
    }
}