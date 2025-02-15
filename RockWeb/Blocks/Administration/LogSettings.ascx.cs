﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

using Rock;
using Rock.Logging;
using Rock.Model;
using Rock.Security;
using Rock.SystemKey;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.Administration
{
    /// <summary>
    /// Used to view the <see cref="Rock.Logging.RockLogEvent"/> logged via RockLogger.
    /// </summary>
    [System.ComponentModel.DisplayName( "Log Settings" )]
    [System.ComponentModel.Category( "Administration" )]
    [System.ComponentModel.Description( "Block to edit rock log settings." )]

    [Rock.SystemGuid.BlockTypeGuid( "6ABC44FD-C4D7-4E30-8537-3A065B493453" )]
    public partial class LogSettings : RockBlock
    {
        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !IsUserAuthorized( Authorization.EDIT ) )
            {
                btnEdit.Visible = false;
                btnDeleteLog.Visible = false;
            }

            if ( !Page.IsPostBack )
            {
                ShowHideEditForm( false, null );
            }
        }
        #endregion

        #region Control Events

        protected void btnLoggingSave_Click( object sender, EventArgs e )
        {
            Page.Validate();
            if ( !Page.IsValid || !IsUserAuthorized( Authorization.EDIT ) )
            {
                return;
            }

            nbLoggingMessage.Visible = true;

            var logConfig = new RockLogSystemSettings
            {
                StandardLogLevel = rblVerbosityLevel.SelectedValue.ConvertToEnum<LogLevel>( LogLevel.None ),
                StandardCategories = rlbCategoriesToLog.SelectedValues,
                IsLocalLoggingEnabled = cbLogToLocal.Checked,
                IsObservabilityLoggingEnabled = cbLogToObservability.Checked,
                AdvancedSettings = ceCustomConfiguration.Text,
                MaxFileSize = nbMaxFileSize.Text.AsInteger(),
                NumberOfLogFiles = nbFilesToRetain.Text.AsInteger()
            };

            Rock.Web.SystemSettings.SetValue( SystemSetting.ROCK_LOGGING_SETTINGS, logConfig.ToJson() );

            RockLogger.ReloadConfiguration();

            ShowHideEditForm( false, logConfig );

            nbLoggingMessage.NotificationBoxType = NotificationBoxType.Success;
            nbLoggingMessage.Title = string.Empty;
            nbLoggingMessage.Text = "Setting saved successfully.";
        }

        protected void btnDeleteLog_Click( object sender, EventArgs e )
        {
            nbLoggingMessage.Visible = true;

            RockLogger.RecycleSerilog();
            ( RockLogger.LogReader as RockSerilogReader ).Delete();

            this.NavigateToCurrentPage();
        }

        protected void btnEdit_Click( object sender, EventArgs e )
        {
            // Hide any previous notifications.
            nbLoggingMessage.Visible = false;

            ShowHideEditForm( true, null );
        }

        protected void btnCancel_Click( object sender, EventArgs e )
        {
            ShowHideEditForm( false, null );
        }

        protected void cbLogToLocal_CheckedChanged( object sender, EventArgs e )
        {
            wpLocalSettings.Visible = cbLogToLocal.Checked;
        }

        #endregion

        #region Internal Methods

        private void BindLoggingSettingsEdit()
        {
            var logLevel = Enum.GetNames( typeof( LogLevel ) );
            rblVerbosityLevel.DataSource = logLevel;
            rblVerbosityLevel.DataBind();

            var rockConfig = Rock.Web.SystemSettings.GetValue( SystemSetting.ROCK_LOGGING_SETTINGS ).FromJsonOrNull<RockLogSystemSettings>();

            var selectedCategories = rockConfig?.StandardCategories ?? new List<string>();
            var categories = new List<string>( RockLogger.GetStandardCategories() );

            foreach ( var category in selectedCategories )
            {
                if ( !categories.Contains( category ) )
                {
                    categories.Add( category );
                }
            }

            rlbCategoriesToLog.DataSource = categories;
            rlbCategoriesToLog.DataBind();

            if ( rockConfig == null )
            {
                return;
            }

            rblVerbosityLevel.SelectedValue = rockConfig.StandardLogLevel.ToString();
            cbLogToLocal.Checked = rockConfig.IsLocalLoggingEnabled;
            cbLogToObservability.Checked = rockConfig.IsObservabilityLoggingEnabled;
            nbFilesToRetain.Text = rockConfig.NumberOfLogFiles.ToString();
            nbMaxFileSize.Text = rockConfig.MaxFileSize.ToString();

            rlbCategoriesToLog.SetValues( selectedCategories );

            wpLocalSettings.Visible = cbLogToLocal.Checked;

            ceCustomConfiguration.Text = rockConfig.AdvancedSettings;
        }

        private void BindLoggingSettingsView( RockLogSystemSettings rockConfig )
        {
            if ( rockConfig == null )
            {
                rockConfig = Rock.Web.SystemSettings.GetValue( SystemSetting.ROCK_LOGGING_SETTINGS ).FromJsonOrNull<RockLogSystemSettings>();
            }

            if ( rockConfig == null )
            {
                return;
            }

            litVerbosityLevel.Text = rockConfig.StandardLogLevel.ToString();

            if ( rockConfig.StandardCategories != null && rockConfig.StandardCategories.Any() )
            {
                litCategories.Text = rockConfig.StandardCategories.JoinStrings( "<br>" );
            }
            else
            {
                litCategories.Text = string.Empty;
            }

            litLocalFileSystem.Text = rockConfig.IsLocalLoggingEnabled ? "Enabled" : "Disabled";
            litObservability.Text = rockConfig.IsObservabilityLoggingEnabled ? "Enabled" : "Disabled";
        }

        private void ShowHideEditForm( bool showEditForm, RockLogSystemSettings rockConfig )
        {
            if ( showEditForm && IsUserAuthorized( Authorization.EDIT ) )
            {
                pnlEditSettings.Visible = true;
                pnlReadOnlySettings.Visible = false;
                HideSecondaryBlocks( true );
                BindLoggingSettingsEdit();
            }
            else
            {
                pnlEditSettings.Visible = false;
                pnlReadOnlySettings.Visible = true;
                HideSecondaryBlocks( false );
                BindLoggingSettingsView( rockConfig );
            }
        }

        #endregion
    }
}