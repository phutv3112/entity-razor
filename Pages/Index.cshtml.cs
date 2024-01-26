using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razorweb.models;

namespace EntityFrame.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly BlogContext _blogContext;

    public IndexModel(ILogger<IndexModel> logger, BlogContext blogContext)
    {
        _logger = logger;
        _blogContext = blogContext;
    }

    public void OnGet()
    {
        var posts = (from a in _blogContext.articles
                     orderby a.Created descending
                     select a).ToList();
        ViewData["posts"] = posts;
    }
}
