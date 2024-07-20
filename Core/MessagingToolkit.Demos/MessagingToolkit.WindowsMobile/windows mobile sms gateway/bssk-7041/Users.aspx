<%@ Page  MasterPageFile="~/MasterPage.master" Title="Users"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Users</h1>
<asp:GridView runat="server" DataSourceID="UsersDataSource" ID="UsersGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="Username"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="Username" SortExpression="Username" HeaderText="Username"></asp:BoundField>
<asp:TemplateField SortExpression="ProorityGroup" HeaderText="ProorityGroup"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllPriorityGroups" ID="PriorityGroup_ProorityGroupDataSource" DataObjectTypeName="bulksms.PriorityGroup" UpdateMethod="Update" TypeName="bulksms.PriorityGroup" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="PriorityGroup_ProorityGroupList" DataTextField="PriorityGroupId" DataValueField="PriorityGroupId" SelectedValue='<%# Bind("ProorityGroup") %>' DataSourceID="PriorityGroup_ProorityGroupDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="ProorityGroupLink" runat="server" Text='<%# Eval("ProorityGroup") %>' NavigateUrl='<%# Eval("ProorityGroup", "~/PriorityGroupDetail.aspx?PriorityGroupId={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Userid" SortExpression="Userid" HeaderText="Userid"></asp:BoundField>
<asp:BoundField DataField="Name" SortExpression="Name" HeaderText="Name"></asp:BoundField>
<asp:BoundField DataField="Country" SortExpression="Country" HeaderText="Country"></asp:BoundField>
<asp:BoundField DataField="City" SortExpression="City" HeaderText="City"></asp:BoundField>
<asp:BoundField DataField="Sex" SortExpression="Sex" HeaderText="Sex"></asp:BoundField>
<asp:BoundField DataField="Birthday" SortExpression="Birthday" HeaderText="Birthday"></asp:BoundField>
<asp:BoundField DataField="Tel" SortExpression="Tel" HeaderText="Tel"></asp:BoundField>
<asp:BoundField DataField="Mobileno" SortExpression="Mobileno" HeaderText="Mobileno"></asp:BoundField>
<asp:BoundField DataField="Mobileno2" SortExpression="Mobileno2" HeaderText="Mobileno2"></asp:BoundField>
<asp:BoundField DataField="Fax" SortExpression="Fax" HeaderText="Fax"></asp:BoundField>
<asp:BoundField DataField="Email" SortExpression="Email" HeaderText="Email"></asp:BoundField>
<asp:BoundField DataField="Website" SortExpression="Website" HeaderText="Website"></asp:BoundField>
<asp:BoundField DataField="Regcode" SortExpression="Regcode" HeaderText="Regcode"></asp:BoundField>
<asp:BoundField DataField="Active" SortExpression="Active" HeaderText="Active"></asp:BoundField>
<asp:BoundField DataField="Points" SortExpression="Points" HeaderText="Points"></asp:BoundField>
<asp:BoundField DataField="Regdate" SortExpression="Regdate" HeaderText="Regdate"></asp:BoundField>
<asp:BoundField DataField="Passcode" SortExpression="Passcode" HeaderText="Passcode"></asp:BoundField>
<asp:BoundField DataField="Lastvisit" SortExpression="Lastvisit" HeaderText="Lastvisit"></asp:BoundField>
<asp:BoundField DataField="Expirydate" SortExpression="Expirydate" HeaderText="Expirydate"></asp:BoundField>
<asp:BoundField DataField="Company" SortExpression="Company" HeaderText="Company"></asp:BoundField>
<asp:BoundField DataField="Timezone" SortExpression="Timezone" HeaderText="Timezone"></asp:BoundField>
<asp:HyperLinkField Text="View GrpMains" DataNavigateUrlFormatString="~/GrpMains.aspx?Table=User_GrpMains&amp;GrpMains_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgHistories" DataNavigateUrlFormatString="~/MsgHistories.aspx?Table=User_MsgHistories&amp;MsgHistories_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgMains" DataNavigateUrlFormatString="~/MsgMains.aspx?Table=User_MsgMains&amp;MsgMains_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgOutboxes" DataNavigateUrlFormatString="~/MsgOutboxes.aspx?Table=User_MsgOutboxes&amp;MsgOutboxes_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View PointHistories" DataNavigateUrlFormatString="~/PointHistories.aspx?Table=User_PointHistories&amp;PointHistories_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View SenderIds" DataNavigateUrlFormatString="~/SenderIds.aspx?Table=User_SenderIds&amp;SenderIds_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/UserDetail.aspx?Username={0}" DataNavigateUrlFields="Username"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetUsersCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetUsers" ID="UsersDataSource" DataObjectTypeName="bulksms.User" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Users_PriorityGroupId" QueryStringField="Users_PriorityGroupId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewUser.aspx">Create New User</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
