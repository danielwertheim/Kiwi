@using Kiwi.Markdown
@model Document

@{
    ViewBag.Title = @Model.Title;
}

<h1>@Model.Title</h1>

<p>@Html.Raw(Model.Content)</p>