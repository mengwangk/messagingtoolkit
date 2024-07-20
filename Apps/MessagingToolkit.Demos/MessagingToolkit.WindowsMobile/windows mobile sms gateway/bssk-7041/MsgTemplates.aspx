<%@ Page  MasterPageFile="~/MasterPage.master" Title="MsgTemplates"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>MsgTemplates</h1>
<asp:GridView runat="server" DataSourceID="MsgTemplatesDataSource" ID="MsgTemplatesGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="Tempid"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="Tempid" SortExpression="Tempid" HeaderText="Tempid"></asp:BoundField>
<asp:BoundField DataField="Userid" SortExpression="Userid" HeaderText="Userid"></asp:BoundField>
<asp:BoundField DataField="Template" SortExpression="Template" HeaderText="Template"></asp:BoundField>
<asp:BoundField DataField="MobileColumn" SortExpression="MobileColumn" HeaderText="MobileColumn"></asp:BoundField>
<asp:BoundField DataField="Type" SortExpression="Type" HeaderText="Type"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/MsgTemplatesDetail.aspx?Tempid={0}" DataNavigateUrlFields="Tempid"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetAllMsgTemplatesCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetAllMsgTemplates" ID="MsgTemplatesDataSource" DataObjectTypeName="bulksms.MsgTemplates" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.MsgTemplates" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewMsgTemplates.aspx">Create New MsgTemplates</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
