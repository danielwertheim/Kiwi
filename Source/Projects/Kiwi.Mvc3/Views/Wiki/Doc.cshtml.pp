@using Kiwi.Markdown
@model Document

@{
    ViewBag.Title = @Model.Title;
}

<h1>@Model.Title</h1>

<div>@Html.Raw(Model.Content)</div>