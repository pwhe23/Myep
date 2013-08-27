
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;

namespace Site
{
	public static class Ext
	{
		public static string Or(this object obj)
		{
			return obj == null ? string.Empty : obj.ToString();
		}

		public static IDictionary<TKey, TValue> Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			dict[key] = value;
			return dict;
		}

		public static HttpResult Created(this Service service, Object model, string location)
		{
			return new HttpResult(model)
			{
				StatusCode = HttpStatusCode.Redirect,
				Headers = {
					{ HttpHeaders.Location, location }
				}
			};
		}
		
		//REF: http://stackoverflow.com/a/15688756/366559
		public static void AlterTable<T>(this IDbConnection db) where T : new()
		{
			var model = ModelDefinition<T>.Definition;

			// just create the table if it doesn't already exist
			if (db.TableExists(model.ModelName) == false)
			{
				db.CreateTable<T>(overwrite: false);
				return;
			}

			// find each of the missing fields
			var columns = GetColumnNames(db, model.ModelName);
			var missing = ModelDefinition<T>.Definition.FieldDefinitions
				.Where(field => columns.Contains(field.FieldName) == false)
				.ToList();

			// add a new column for each missing field
			foreach (var field in missing)
			{
				var alterSql = string.Format("ALTER TABLE {0} ADD {1} {2}",
					model.ModelName,
					field.FieldName,
					db.GetDialectProvider().GetColumnTypeDefinition(field.FieldType)
				);
				LogManager.GetLogger(typeof(Ext)).Debug(alterSql);
				db.ExecuteSql(alterSql);
			}
		}

		private static List<string> GetColumnNames(IDbConnection db, string tableName)
		{
			var columns = new List<string>();
			using (var cmd = db.CreateCommand())
			{
				cmd.CommandText = "exec sp_columns " + tableName;
				var reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					var ordinal = reader.GetOrdinal("COLUMN_NAME");
					columns.Add(reader.GetString(ordinal));
				}
				reader.Close();
			}
			return columns;
		}

		public static bool MapData<TModel>(this Controller controller, TModel model) where TModel : class
		{
			return MapData(controller, model, null, null, null, null);
		}

		public static bool MapData<TModel>(this Controller controller, TModel model, string prefix, string[] includeProperties, string[] excludeProperties, IValueProvider valueProvider) where TModel : class
		{
			if (model == null) throw new ArgumentNullException("model");
			if (valueProvider == null) valueProvider = ValueProviderFactories.Factories.GetValueProvider(controller.ControllerContext);
			var binders = ModelBinders.Binders;
			var binder = binders.GetBinder(typeof(TModel));
			var bindingContext = new ModelBindingContext();
			bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(TModel));
			bindingContext.ModelName = prefix;
			bindingContext.ModelState = controller.ViewData.ModelState;
			bindingContext.PropertyFilter = propertyName => IsPropertyAllowed(propertyName, includeProperties, excludeProperties);
			bindingContext.ValueProvider = valueProvider;
			binder.BindModel(controller.ControllerContext, bindingContext);
			//if (model is IValidatableObject) {
			//	var list = new List<ValidationResult>();
			//	Validator.TryValidateObject(model, new ValidationContext(model, null, null), list, true);
			//	foreach (var item in list) {
			//		foreach (var member in item.MemberNames) {
			//			ViewData.ModelState.AddModelError(member, item.ErrorMessage);
			//		}
			//	}
			//}
			return controller.ViewData.ModelState.IsValid;
		}

		private static bool IsPropertyAllowed(string propertyName, string[] includeProperties, string[] excludeProperties)
		{
			var flag = ((includeProperties == null) || (includeProperties.Length == 0)) || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
			var flag2 = (excludeProperties != null) && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
			return (flag && !flag2);
		}
	};
}
