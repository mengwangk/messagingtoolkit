<%@ Page  MasterPageFile="~/MasterPage.master" Title="New User" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New User</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="UsersDataSource" EmptyDataText="The requested record was not found." ID="UsersDetailsView" AutoGenerateRows="False" DataKeyNames="Username"><Fields>
<asp:BoundField ReadOnly="True" DataField="Username" HeaderText="Username"></asp:BoundField>
<asp:TemplateField SortExpression="ProorityGroup" HeaderText="ProorityGroup"><InsertItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllPriorityGroups" ID="PriorityGroup_ProorityGroupDataSource" DataObjectTypeName="bulksms.PriorityGroup" UpdateMethod="Update" TypeName="bulksms.PriorityGroup" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="PriorityGroup_ProorityGroupList" DataTextField="PriorityGroupId" DataValueField="PriorityGroupId" SelectedValue='<%# Bind("ProorityGroup") %>' DataSourceID="PriorityGroup_ProorityGroupDataSource" >
</asp:DropDownList>
</InsertItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Userid" HeaderText="Userid"></asp:BoundField>
<asp:BoundField DataField="Name" HeaderText="Name"></asp:BoundField>
<asp:BoundField DataField="Country" HeaderText="Country"></asp:BoundField>
<asp:BoundField DataField="City" HeaderText="City"></asp:BoundField>
<asp:BoundField DataField="Sex" HeaderText="Sex"></asp:BoundField>
<asp:BoundField DataField="Birthday" HeaderText="Birthday"></asp:BoundField>
<asp:BoundField DataField="Tel" HeaderText="Tel"></asp:BoundField>
<asp:BoundField DataField="Mobileno" HeaderText="Mobileno"></asp:BoundField>
<asp:BoundField DataField="Mobileno2" HeaderText="Mobileno2"></asp:BoundField>
<asp:BoundField DataField="Fax" HeaderText="Fax"></asp:BoundField>
<asp:BoundField DataField="Email" HeaderText="Email"></asp:BoundField>
<asp:BoundField DataField="Website" HeaderText="Website"></asp:BoundField>
<asp:BoundField DataField="Regcode" HeaderText="Regcode"></asp:BoundField>
<asp:BoundField DataField="Active" HeaderText="Active"></asp:BoundField>
<asp:BoundField DataField="Points" HeaderText="Points"></asp:BoundField>
<asp:BoundField DataField="Regdate" HeaderText="Regdate"></asp:BoundField>
<asp:BoundField DataField="Passcode" HeaderText="Passcode"></asp:BoundField>
<asp:BoundField DataField="Lastvisit" HeaderText="Lastvisit"></asp:BoundField>
<asp:BoundField DataField="Expirydate" HeaderText="Expirydate"></asp:BoundField>
<asp:BoundField DataField="Company" HeaderText="Company"></asp:BoundField>
<asp:BoundField DataField="Timezone" HeaderText="Timezone"></asp:BoundField>
<asp:HyperLinkField Text="View GrpMains" DataNavigateUrlFormatString="~/GrpMains.aspx?Table=User_GrpMains&amp;GrpMains_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgHistories" DataNavigateUrlFormatString="~/MsgHistories.aspx?Table=User_MsgHistories&amp;MsgHistories_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgMains" DataNavigateUrlFormatString="~/MsgMains.aspx?Table=User_MsgMains&amp;MsgMains_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgOutboxes" DataNavigateUrlFormatString="~/MsgOutboxes.aspx?Table=User_MsgOutboxes&amp;MsgOutboxes_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View PointHistories" DataNavigateUrlFormatString="~/PointHistories.aspx?Table=User_PointHistories&amp;PointHistories_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View SenderIds" DataNavigateUrlFormatString="~/SenderIds.aspx?Table=User_SenderIds&amp;SenderIds_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetUser" ID="UsersDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Username" QueryStringField="Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
