﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AttendanceDetail.ascx.cs" Inherits="RockWeb.Blocks.CheckIn.Manager.AttendanceDetail" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <!-- Attendance Details -->
        <div class="panel panel-body">
            <div class="row">
                <div class="col-sm-6">
                    <Rock:RockLiteral ID="lGroupName" runat="server" Label="Group" />
                    <Rock:RockLiteral ID="lLocationName" runat="server" Label="Location" />
                </div>
                <div class="col-sm-6">
                    <Rock:RockLiteral ID="lTag" runat="server" Label="Tag" />
                    <Rock:RockLiteral ID="lScheduleName" runat="server" Label="Schedule" />
                </div>
            </div>

            <hr />

            <div class="row">
                <div class="col-md-6">
                    <asp:Panel ID="pnlCheckinTimeDetails" runat="server">
                        <Rock:RockLiteral ID="lCheckinTime" runat="server" Label="Check-in" />
                    </asp:Panel>

                    <asp:Panel ID="pnlPresentDetails" runat="server">
                        <Rock:RockLiteral ID="lPresentTime" runat="server" Label="Present" />
                    </asp:Panel>

                    <asp:Panel ID="pnlCheckedOutDetails" runat="server">
                        <Rock:RockLiteral ID="lCheckedOutTime" runat="server" Label="Checked-out" />
                    </asp:Panel>
                </div>
                <div class="col-md-6">
                    <Rock:RockLiteral runat="server" Label="Change History"/>
                    <ul>
                        <asp:Repeater ID="rptHistoryList" runat="server">
                            <ItemTemplate>
                                <li></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </div>

            <div class="actions">
                <asp:LinkButton ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click" />
                <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-link" OnClick="btnDelete_Click" />
            </div>
        </div>

        <Rock:ModalDialog ID="mdMovePerson" runat="server" Title="Edit Attendance" SaveButtonText="Move" OnSaveClick="mdMovePerson_SaveClick">
            <Content>
                 <asp:Panel ID="pnlCheckInCheckOutEdit" runat="server" class="row" Visible="false">
                     <div class="col-md-6">
                         <Rock:DateTimePicker ID="dtpStart" runat="server" SourceTypeName="Rock.Model.Attendance" PropertyName="StartDateTime" Required="false" Label="Check-in Date/Time" />
                     </div>
                     <div class="col-md-6">
                         <Rock:DateTimePicker ID="dtpEnd" runat="server" SourceTypeName="Rock.Model.Attendance" PropertyName="EndDateTime" Required="false" Label="Check-out Date/Time" />
                     </div>
                </asp:Panel>
                <div class="well">
                    <h4>Move Individual</h4>
                    <div class="row">
                        <div class="col-md-4">
                            <Rock:RockDropDownList ID="ddlMovePersonSchedule" runat="server" Label="Schedule" AutoPostBack="true" Required="true" OnSelectedIndexChanged="ddlMovePersonSchedule_SelectedIndexChanged" />
                        </div>
                        <div class="col-md-4">
                            <Rock:RockDropDownList ID="ddlMovePersonLocation" runat="server" Label="Location" AutoPostBack="true" Required="true" OnSelectedIndexChanged="ddlMovePersonLocation_SelectedIndexChanged" />
                        </div>
                        <div class="col-md-4">
                            <Rock:RockDropDownList ID="ddlMovePersonGroup" runat="server" Label="Group" AutoPostBack="true" Required="true" />
                        </div>
                    </div>
                </div>

                <Rock:NotificationBox ID="nbMovePersonLocationFull" runat="server" NotificationBoxType="Warning" />

            </Content>
        </Rock:ModalDialog>
    </ContentTemplate>
</asp:UpdatePanel>
