using System;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Areas.Admin.ViewModels.Banners;
using Mebeller.Areas.Admin.ViewModels.Messages;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class MediaController : Controller
{
    private readonly IMediaService _mediaService;
    public MediaController(IMediaService mediaService) => _mediaService = mediaService;

    [HttpGet("/Admin/Messages")]
    public async Task<IActionResult> Messages(int pageNumber = 1, string search = null)
    {
        var messages = await _mediaService.GetMessagesAsync();

        messages = !string.IsNullOrEmpty(search)
            ? messages
                .Where(p => p.MessageSenderName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            p.MessageDescription.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            p.MessageSubject.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            p.MessageSenderEmail.Contains(search, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.IsRead)
            : messages.OrderBy(p => p.IsRead);

        if (!messages.Any())
        {
            ViewData["isEmpty"] = true;
            return View();
        }

        var page = new Paging<Message>(messages, 6, pageNumber);
        if (pageNumber < page.FirstPage || pageNumber > page.LastPage)
        {
            return NotFound();
        }

        var messagesPage = page.QueryResult;
        ViewData["pageNumber"] = pageNumber;
        ViewData["firstPage"] = page.FirstPage;
        ViewData["lastPage"] = page.LastPage;
        ViewData["prevPage"] = page.PreviousPage;
        ViewData["nextPage"] = page.NextPage;
        ViewData["search"] = search;
        ViewData["isEmpty"] = false;
        return View(messagesPage);
    }


    [HttpGet("/Admin/Messages/{messageId}")]
    public async Task<IActionResult> MessageDetails(int messageId)
    {
        var message = await _mediaService.GetMessageAsync(messageId);
        var model = new MessageDetailViewModel()
        {
            Message = message,
            MessageReplyDescription = message.MessageReply?.MessageReplyDescription,
            MessageSubmitTime = message.SubmitTime
        };
        await _mediaService.SetMessageAsReadAsync(message);
        return View(model);
    }

    [HttpPost("/Admin/Messages/ReplyToMessage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReplyToMessage(MessageDetailViewModel model)
    {
        var messageId = Convert.ToInt32(TempData["messageId"]);
        var result = await _mediaService.ReplyToMessageAsync(model.MessageReplyDescription, messageId);
        var message = await _mediaService.GetMessageAsync(messageId);
        model.Message = message;
        if (result)
        {
            ViewData["SuccessMessage"] = "Your message was sent successfully.";
            ModelState.Clear();
            model.MessageSubmitTime = message.MessageReply.MessageReplySubmitTime;
            return View("MessageDetails", model);
        }

        ModelState.AddModelError("", "A problem occurred while sending the message.");
        TempData.Keep("messageId");
        return View("MessageDetails", model);
    }

    [HttpGet("/Admin/Messages/DeleteMessage")]
    public async Task<IActionResult> DeleteMessage(int messageId) =>
        await _mediaService.DeleteMessageByIdAsync(messageId) ? RedirectToAction("Messages") : NotFound();

    [HttpGet("/Admin/Comments")]
    public async Task<IActionResult> Comments(int pageNumber = 1, string search = null)
    {
        var comments = await _mediaService.GetCommentsAsync();

        comments = !string.IsNullOrEmpty(search)
            ? comments
                .Where(p => p.CommentDescription.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            p.User.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            p.Product.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.IsRead)
            : comments.OrderBy(p => p.IsRead);

        if (!comments.Any())
        {
            ViewData["isEmpty"] = true;
            return View();
        }

        var page = new Paging<Comment>(comments, 6, pageNumber);
        if (pageNumber < page.FirstPage || pageNumber > page.LastPage)
        {
            return NotFound();
        }

        var commentsPage = page.QueryResult;
        ViewData["pageNumber"] = pageNumber;
        ViewData["firstPage"] = page.FirstPage;
        ViewData["lastPage"] = page.LastPage;
        ViewData["prevPage"] = page.PreviousPage;
        ViewData["nextPage"] = page.NextPage;
        ViewData["search"] = search;
        ViewData["isEmpty"] = false;
        return View(commentsPage);
    }


    [HttpGet("/Admin/Comments/{commentId}")]
    public async Task<IActionResult> CommentDetails(int commentId)
    {
        var comment = await _mediaService.GetCommentAsync(commentId);
        if (comment == null)
        {
            return NotFound();
        }

        await _mediaService.SetCommentAsReadAsync(comment);
        return View(comment);
    }

    [HttpPost("/Admin/Comments/ReplyToCommentByAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReplyToCommentByAdmin(string NewCommentReplyDescription)
    {
        var commentId = Convert.ToInt32(TempData["commentId"]);
        var result = await _mediaService.AddReplyToCommentByAdminAsync(commentId, NewCommentReplyDescription);
        if (result)
        {
            ViewData["SuccessMessageForAddCommentReply"] = "Your response has been successfully saved";
            var comment = await _mediaService.GetCommentAsync(commentId);
            return View("CommentDetails", comment);
        }

        return NotFound();
    }

    [HttpPost("/Admin/Comments/EditComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditComment(string NewCommentDescription)
    {
        var commentId = Convert.ToInt32(TempData["commentId"]);
        var comment = await _mediaService.GetCommentAsync(commentId);
        comment.CommentDescription = NewCommentDescription;
        var result = await _mediaService.UpdateCommentAsync(comment);
        if (result)
        {
            ViewData["SuccessMessageForEditComment"] = "The comment was successfully edited";
            return View("CommentDetails", comment);
        }

        return NotFound();
    }

    [HttpGet("/Admin/Comments/ConfirmComment")]
    public async Task<IActionResult> ConfirmComment(int commentId, string returnUrl)
    {
        var result = await _mediaService.ConfirmCommentAsync(commentId);
        switch (result)
        {
            case true:
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = Url.Action("Comments");
                }

                return returnUrl != null ? Redirect(returnUrl) : NotFound();
            }
            default:
                return NotFound();
        }
    }

    [HttpGet("/Admin/Comments/RejectComment")]
    public async Task<IActionResult> RejectComment(int commentId, string returnUrl)
    {
        var result = await _mediaService.RejectCommentAsync(commentId);
        switch (result)
        {
            case true:
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = Url.Action("Comments");
                }

                return returnUrl != null ? Redirect(returnUrl) : NotFound();
            }
            default:
                return NotFound();
        }
    }

    [HttpGet("/Admin/Comments/DeleteComment")]
    public async Task<IActionResult> DeleteComment(int commentId, string returnUrl = null)
    {
        var result = await _mediaService.DeleteCommentByIdAsync(commentId);
        switch (result)
        {
            case true:
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = Url.Action("Comments");
                }

                return returnUrl != null ? Redirect(returnUrl) : NotFound();
            }
            default:
                return NotFound();
        }
    }

    [HttpGet("/Admin/Banners")]
    public async Task<IActionResult> Banners(int pageNumber = 1, string search = null)
    {
        var banners = await _mediaService.GetBannersAsync();

        banners = string.IsNullOrEmpty(search) switch
        {
            false => banners.Where(p =>
                p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(search, StringComparison.OrdinalIgnoreCase) || p.Id.ToString() == search),
            _ => banners
        };

        if (!banners.Any())
        {
            ViewData["isEmpty"] = true;
            return View();
        }

        var page = new Paging<Banner>(banners, 6, pageNumber);
        if (pageNumber < page.FirstPage || pageNumber > page.LastPage)
        {
            return NotFound();
        }

        var bannersPage = page.QueryResult;
        ViewData["pageNumber"] = pageNumber;
        ViewData["firstPage"] = page.FirstPage;
        ViewData["lastPage"] = page.LastPage;
        ViewData["prevPage"] = page.PreviousPage;
        ViewData["nextPage"] = page.NextPage;
        ViewData["search"] = search;
        ViewData["isEmpty"] = false;
        return View(bannersPage);
    }

    [HttpGet("/Admin/Banners/AddBanner")]
    public IActionResult AddBanner() => View();

    [HttpPost("/Admin/Banners/AddBanner")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBanner(AddBannerViewModel model)
    {
        switch (ModelState.IsValid)
        {
            case true:
            {
                var result = await _mediaService.AddBannerAsync(model);
                if (result)
                {
                    return RedirectToAction("Banners");
                }

                ModelState.AddModelError("", "A problem occurred while adding the banner");
                break;
            }
        }

        return View(model);
    }

    [HttpGet("/Admin/Banners/EditBanner")]
    public async Task<IActionResult> EditBanner(int bannerId) =>
        await _mediaService.GetBannerForEditAsync(bannerId) == null
            ? NotFound()
            : View(await _mediaService.GetBannerForEditAsync(bannerId));

    [HttpPost("/Admin/Banners/EditBanner")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBanner(EditBannerViewModel model)
    {
        switch (ModelState.IsValid)
        {
            case true:
            {
                var result = await _mediaService.UpdateBannerAsync(model);
                if (result)
                {
                    return RedirectToAction("Banners");
                }

                ModelState.AddModelError("", "A problem occurred while editing the banner");
                break;
            }
        }

        return View(model);
    }

    [HttpGet("/Admin/Banners/DeleteBanner")]
    public async Task<IActionResult> DeleteBanner(int bannerId) =>
        await _mediaService.DeleteBannerByIdAsync(bannerId) ? RedirectToAction("Banners") : NotFound();

    [HttpGet("/Admin/Newsletters")]
    public async Task<IActionResult> Newsletters(int pageNumber = 1, string search = null)
    {
        var newsletters = await _mediaService.GetNewslettersAsync();

        if (!string.IsNullOrEmpty(search))
        {
            newsletters = newsletters.Where(p =>
                p.CustomerEmail.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.NewsletterId.ToString() == search);
        }

        if (!newsletters.Any())
        {
            ViewData["isEmpty"] = true;
            return View();
        }

        var page = new Paging<Newsletter>(newsletters, 6, pageNumber);
        if (pageNumber < page.FirstPage || pageNumber > page.LastPage)
        {
            return NotFound();
        }

        var newslettersPage = page.QueryResult;
        ViewData["pageNumber"] = pageNumber;
        ViewData["firstPage"] = page.FirstPage;
        ViewData["lastPage"] = page.LastPage;
        ViewData["prevPage"] = page.PreviousPage;
        ViewData["nextPage"] = page.NextPage;
        ViewData["search"] = search;
        ViewData["isEmpty"] = false;
        return View(newslettersPage);
    }


    [HttpGet("/Admin/Newsletters/AddNewsletter")]
    public IActionResult AddNewsletter() => View();

    [HttpPost("/Admin/Newsletters/AddNewsletter")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddNewsletter(AddNewsletterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (await _mediaService.AddNewsletterAsync(model.CustomerEmail))
            {
                return RedirectToAction("Newsletters");
            }

            ModelState.AddModelError("", "A problem occurred while registering an email");
        }

        return View(model);
    }

    [HttpGet("/Admin/Newsletters/DeleteNewsletter")]
    public async Task<IActionResult> DeleteNewsletter(int newsletterId) =>
        await _mediaService.DeleteNewsletterByIdAsync(newsletterId) ? RedirectToAction("Newsletters") : NotFound();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IsNewsletterEmailExist(string customerEmail) =>
        !await _mediaService.IsEmailExistInNewslettersCustomersAsync(customerEmail)
            ? new JsonResult(true)
            : new JsonResult("Previously registered email");
}