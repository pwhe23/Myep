
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Site
{
	public static class HtmlHelpers
	{
		public static MvcHtmlString ReadOnly(this HtmlHelper htmlHelper, object value, IDictionary<string, object> htmlAttributes = null)
		{
			var tagBuilder = new TagBuilder("span");
			tagBuilder.MergeAttributes(htmlAttributes);
			tagBuilder.SetInnerText(value.Or());
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
		}

		public static ModelMetadata Metadata<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string displayName = null)
		{
			var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			if (displayName != null) metadata.DisplayName = displayName;
			return metadata;
		}

		public static MvcHtmlString Editor<TModel>(this HtmlHelper<TModel> htmlHelper, ModelMetadata metadata, IDictionary<string, object> htmlAttributes = null)
		{
			//.Set("placeholder", metadata.DisplayName)
			if (metadata.PropertyName == "Id")
			{
				return htmlHelper.ReadOnly(metadata.Model, htmlAttributes);
			}
			else if (metadata.ModelType == typeof(bool?))
			{
				return htmlHelper.CheckBox(metadata.PropertyName, (bool?)metadata.Model == true, htmlAttributes);
			}
			else
			{
				return htmlHelper.TextBox(metadata.PropertyName, metadata.Model, htmlAttributes);
			}
		}
	};
}
