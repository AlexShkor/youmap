﻿using System;
using System.Web.Mvc;

#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YouMap.Framework.Mvc.RazorHelpers
{
    #line 2 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"
#line default
    #line hidden

    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.2.0.0")]
    public static class MoneyHelper
    {

public static System.Web.WebPages.HelperResult Money(this HtmlHelper helper, long value, bool includeDollarSign = false)
{
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {



#line 5 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"
 

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, "    <span class=\'");



#line 6 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"
WebViewPage.WriteTo(@__razor_helper_writer, value < 0 ? "amount red" : String.Empty);

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, " \'>");



#line 6 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"
                    WebViewPage.WriteTo(@__razor_helper_writer, includeDollarSign ? (value/100.0).ToString("C2") : (value/100.0).ToString("C2")
#line default
#line hidden



#line 6 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"
                                                                                                                                                                              );

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, "</span>\r\n");



#line 7 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"

#line default
#line hidden

});

}


public static System.Web.WebPages.HelperResult MoneyColored(this HtmlHelper helper, long value, bool includeDollarSign = false, string cssClass = "amount")
{
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {



#line 10 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"
 

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, "    <span class=\'");



#line 11 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"
WebViewPage.WriteTo(@__razor_helper_writer, value < 0 ? "amount red" : cssClass);

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, " \'>");



#line 11 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"
                WebViewPage.WriteTo(@__razor_helper_writer, includeDollarSign ? (value / 100.0).ToString("C2") : (value / 100.0).ToString("C2")
#line default
#line hidden



#line 11 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"
                                                                                                                                                                              );

#line default
#line hidden

WebViewPage.WriteLiteralTo(@__razor_helper_writer, "</span>\r\n");



#line 12 "..\..\Mvc\RazorHelpers\MoneyHelper.cshtml"

#line default
#line hidden

});

}


    }
}
#pragma warning restore 1591
