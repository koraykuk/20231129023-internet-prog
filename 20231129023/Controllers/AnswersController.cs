using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _20231129023.Models;
using _20231129023.ViewModels;

namespace _20231129023.Controllers
{
    public class AnswersController : Controller
    {
        private readonly AppDbContext _context;

        public AnswersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Answers
        public async Task<IActionResult> Index()
        {
              return _context.Answers != null ? 
                          View(await _context.Answers.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Answers'  is null.");
        }

        public async Task<IActionResult> Index(int? id)
        {
            Question question = _context.Questions.SingleOrDefault(q => q.Id == id);
            return View(new QuestionDetails()
            {
                QuestionId = question.Id,
                UserId = question.UserId,
                QuestionDate = question.Date,
                QuestionText = question.QuestionText,
                QuestionTitle = question.QuestionTitle,
                Answers = await _context.Answers.ToListAsync(),
            });
        }

        // GET: Answers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Question question = _context.Questions.SingleOrDefault(q => q.Id == id);
            if (question == null) return Redirect("Details/"+id);
            var x = _context.Answers.Where(x => x.QuestionId == question.Id).ToList();

            return View(new QuestionDetails()
            {
                QuestionId = question.Id,
                UserId = question.UserId,
                QuestionDate = question.Date,
                QuestionText = question.QuestionText,
                QuestionTitle = question.QuestionTitle,
                Answers =  x
            });
        }

        // GET: Answers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QuestionId,UserId,AnswerText,Date")] Answer answer)
        {
            answer.Date = DateTime.Now;
            await _context.AddAsync(answer);
            await _context.SaveChangesAsync();
            return Redirect("Details/"+answer.QuestionId);
        }

        // GET: Answers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Answers == null)
            {
                return NotFound();
            }

            var answer = await _context.Answers.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionId,UserId,AnswerText,Date")] Answer answer)
        {
            if (id != answer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(answer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnswerExists(answer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(answer);
        }

        // GET: Answers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Answers == null)
            {
                return NotFound();
            }

            var answer = await _context.Answers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Answers == null)
            {
                return Problem("Entity set 'AppDbContext.Answers'  is null.");
            }
            var answer = await _context.Answers.FindAsync(id);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnswerExists(int id)
        {
          return (_context.Answers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
