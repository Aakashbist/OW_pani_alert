using Microsoft.Data.SqlClient;
using HT.WaterAlerts.Domain;
using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace HT.WaterAlerts.Service
{
    public static class ServiceHelper
    {
        public static Expression<Func<T, bool>> GetFilterPredicate<T>(string stringExpression)
        {
            var param = Expression.Parameter(typeof(T), @"x");
            var exp = DynamicExpressionParser.ParseLambda(new[] { param }, null, stringExpression);
            return (Expression<Func<T, bool>>)exp;
        }

        public static Expression<Func<T, object>> GetOrderPredicate<T>(string orderByString)
        {
            var parameter = Expression.Parameter(typeof(T));
            string[] properties = orderByString.Split(".");
            Expression lastMember = parameter;
            
            // Getting property for embeded object (AlertLevels.MeasurementSite.Name)
            for (int i = 0; i < properties.Length; i++)
            {
                lastMember = Expression.Property(lastMember, properties[i]);
            }

            var propAsObject = Expression.Convert(lastMember, typeof(object));
            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        public static (string Filter, string OrderColumn, SortOrder OrderDirection) GetDataTableParams(DataTableRequestDTO request)
        {
            string filterString = "True";
            var orderByString = "Id";
            var orderDir = SortOrder.Ascending;
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.SearchColumn) && !string.IsNullOrEmpty(request.SearchValue))
                {
                    var searchValue = request.SearchValue.Replace("\"", "\\\"");
                    filterString += " AND " + request.SearchColumn + ".Contains(\"" + searchValue + "\")";
                }

                if (!string.IsNullOrEmpty(request.OrderColumn))
                {
                    orderByString = request.OrderColumn;
                    if (request.OrderDirection.ToLower() == "desc")
                    {
                        orderDir = SortOrder.Descending;
                    }
                }
            }
            return (filterString, orderByString, orderDir);
        }
    }
}
