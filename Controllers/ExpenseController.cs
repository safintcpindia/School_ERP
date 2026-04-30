using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Services.Clients;
using SchoolERP.Net.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This controller manages the school's expenses, including recording new costs and managing different types of expense categories.
    /// </summary>
    public class ExpenseController : Controller
    {
        private readonly IAccountHeadClientService _headClient;
        private readonly IAccountEntryClientService _entryClient;

        public ExpenseController(IAccountHeadClientService headClient, IAccountEntryClientService entryClient)
        {
            _headClient = headClient;
            _entryClient = entryClient;
        }

        /// <summary>
        /// Shows the main expense recording page, displaying recent expenses and allowing you to pick a category for new ones.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Step 1: Ask the system for all recorded expenses and all expense categories (heads).
            var resEntries = await _entryClient.GetAllAccountEntriesAsync("Expense");
            var resHeads = await _headClient.GetAllAccountHeadsAsync("Expense");

            if (!resEntries.Success) ViewBag.ErrorMessage = resEntries.Message;

            // Step 2: Organize the data to be shown on the expense management page.
            var model = new AccountEntryPageViewModel
            {
                Items = resEntries.Success ? resEntries.Data : new List<AccountEntryViewModel>(),
                Heads = resHeads.Success ? resHeads.Data : new List<AccountHeadViewModel>(),
                EntryType = "Expense"
            };
            
            // Step 3: Open the 'Expense' page.
            return View(model);
        }


        public async Task<IActionResult> ExpenseHead()
        {
            var res = await _headClient.GetAllAccountHeadsAsync("Expense");
            var model = new AccountHeadPageViewModel
            {
                Items = res.Success ? res.Data : new List<AccountHeadViewModel>(),
                HeadType = "Expense"
            };
            return View(model);
        }
        public IActionResult Search()
        {
            var model = new AccountEntrySearchViewModel { EntryType = "Expense" };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Search(AccountEntrySearchRequest req)
        {
            var res = await _entryClient.SearchAccountEntriesAsync(req);
            var model = new AccountEntrySearchViewModel
            {
                Results = res.Success ? res.Data : new List<AccountEntryViewModel>(),
                EntryType = req.EntryType,
                SearchType = req.SearchType,
                DateFrom = req.DateFrom,
                DateTo = req.DateTo
            };
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
            return View(model);
        }
    }
}
