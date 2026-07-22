using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.DTOs.Products
{
    public class CreateCommentDto
    {
        public int ProductID { get; set; }
        public int? ParentID { get; set; }
        public string Text { get; set; }
    }
    public class EditCommentDto
    {
        public int CommentID { get; set; }
        public string Text { get; set; }
    }
    public class ProductCommentViewModel
    {
        public int CommentID { get; set; }
        public string UserFullName { get; set; }
        public string RoleTitle { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEdited { get; set; }
        public bool IsOwner { get; set; }
        public bool CanDelete { get; set; }
        public List<ProductCommentViewModel> Replies { get; set; } = new();
    }
}
