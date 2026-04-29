using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Services.Clients;
using SchoolERP.Net.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IAccountHeadClientService _headClient;
        private readonly IAccountEntryClientService _entryClient;

        public ExpenseController(IAccountHeadClientService headClient, IAccountEntryClientService entryClient)
        {
            _headClient = headClient;
            _entryClient = entryClient;
        }

        public async Task<IActionResult> Index()
        {
            var resEntries = await _entryClient.GetAllAccountEntriesAsync("Expense");
            var resHeads = await _headClient.GetAllAccountHeadsAsync("Expense");

            if (!resEntries.Success) ViewBag.ErrorMessage = resEntries.Message;

            var model = new AccountEntryPageViewModel
            {
                Items = resEntries.Success ? resEntries.Data : new List<AccountEntryViewModel>(),
                Heads = resHeads.Success ? resHeads.Data : new List<AccountHeadViewModel>(),
                EntryType = "Expense"
            };
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
