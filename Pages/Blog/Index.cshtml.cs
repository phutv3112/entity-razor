using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razorweb.models;

namespace EntityFrame.Pages_Blog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly razorweb.models.BlogContext _context;

        public IndexModel(razorweb.models.BlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get; set; } = default!;
        public const int ITEM_PER_PAGE = 5;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { set; get; }
        public int countPages { set; get; }


        public async Task OnGetAsync(string search)
        {
            int totalArticles = await _context.articles.CountAsync();
            countPages = (int)Math.Ceiling((double)totalArticles / ITEM_PER_PAGE);
            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPages) currentPage = countPages;
            // Article = await _context.articles.ToListAsync();
            var query = (from a in _context.articles
                         orderby a.Created descending
                         select a).Skip((currentPage - 1) * ITEM_PER_PAGE)
                        .Take(ITEM_PER_PAGE);
            if (!string.IsNullOrEmpty(search))
            {
                Article = query.Where(a => a.Title.Contains(search)).ToList();
            }
            else
            {
                Article = await query.ToListAsync();
            }

        }
    }
}
