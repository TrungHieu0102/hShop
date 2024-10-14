using System;
using System.ComponentModel;

namespace Core.SeedWorks.Constants
{
    public static class Permissions
    {
        public static class Dashboard
        {
            [Description("Xem dashboard")]
            public const string View = "Permissions.Dashboard.View";
        }

        public static class Products
        {
            [Description("Xem sản phẩm")]
            public const string View = "Permissions.Products.View";
            [Description("Tạo sản phẩm")]
            public const string Create = "Permissions.Products.Create";
            [Description("Sửa sản phẩm")]
            public const string Edit = "Permissions.Products.Edit";
            [Description("Xóa sản phẩm")]
            public const string Delete = "Permissions.Products.Delete";
            [Description("Quản lý hàng tồn kho")]
            public const string ManageInventory = "Permissions.Products.ManageInventory";
        }

        public static class Categories
        {
            [Description("Xem danh mục sản phẩm")]
            public const string View = "Permissions.Categories.View";
            [Description("Tạo danh mục sản phẩm")]
            public const string Create = "Permissions.Categories.Create";
            [Description("Sửa danh mục sản phẩm")]
            public const string Edit = "Permissions.Categories.Edit";
            [Description("Xóa danh mục sản phẩm")]
            public const string Delete = "Permissions.Categories.Delete";
        }

        public static class Orders
        {
            [Description("Xem đơn hàng")]
            public const string View = "Permissions.Orders.View";
            [Description("Tạo đơn hàng")]
            public const string Create = "Permissions.Orders.Create";
            [Description("Sửa đơn hàng")]
            public const string Edit = "Permissions.Orders.Edit";
            [Description("Xóa đơn hàng")]
            public const string Delete = "Permissions.Orders.Delete";
            [Description("Quản lý trạng thái đơn hàng")]
            public const string ManageStatus = "Permissions.Orders.ManageStatus";
        }

        public static class Customers
        {
            [Description("Xem khách hàng")]
            public const string View = "Permissions.Customers.View";
            [Description("Tạo khách hàng")]
            public const string Create = "Permissions.Customers.Create";
            [Description("Sửa thông tin khách hàng")]
            public const string Edit = "Permissions.Customers.Edit";
            [Description("Xóa khách hàng")]
            public const string Delete = "Permissions.Customers.Delete";
        }

        public static class Reports
        {
            [Description("Xem báo cáo doanh thu")]
            public const string ViewRevenue = "Permissions.Reports.ViewRevenue";
            [Description("Xem báo cáo tồn kho")]
            public const string ViewInventory = "Permissions.Reports.ViewInventory";
        }

        public static class Roles
        {
            [Description("Xem quyền")]
            public const string View = "Permissions.Roles.View";
            [Description("Tạo mới quyền")]
            public const string Create = "Permissions.Roles.Create";
            [Description("Sửa quyền")]
            public const string Edit = "Permissions.Roles.Edit";
            [Description("Xóa quyền")]
            public const string Delete = "Permissions.Roles.Delete";
        }

        public static class Users
        {
            [Description("Xem người dùng")]
            public const string View = "Permissions.Users.View";
            [Description("Tạo người dùng")]
            public const string Create = "Permissions.Users.Create";
            [Description("Sửa người dùng")]
            public const string Edit = "Permissions.Users.Edit";
            [Description("Xóa người dùng")]
            public const string Delete = "Permissions.Users.Delete";
        }
    }
}
