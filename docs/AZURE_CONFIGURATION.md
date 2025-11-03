# Azure Configuration Guide

## Required App Settings

To complete the Application Insights setup and secure email configuration, configure the following settings in the Azure Portal.

### How to Configure

1. Go to **Azure Portal** → **App Services** → **OcelisConfigurator**
2. Navigate to **Settings** → **Configuration** → **Application settings**
3. Add the following settings:

---

## Application Insights

### APPLICATIONINSIGHTS_CONNECTION_STRING

**Required:** Yes

Get this from Azure Portal:
1. Go to **Application Insights** resource (or create one)
2. Navigate to **Overview** → **Connection String**
3. Copy the full connection string

```
APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx;IngestionEndpoint=https://...
```

---

## Email Configuration

### Email__SmtpUsername

**Required:** Yes
**Current placeholder:** `YOUR_MAILBOX_USER_HERE`

Your SMTP authentication username for `wes1-smtp.wedos.net`

```
Email__SmtpUsername=your-actual-username
```

### Email__SmtpPassword

**Required:** Yes
**Current placeholder:** `YOUR_PASSWORD_HERE`
**Security:** Consider using Azure Key Vault for sensitive credentials

Your SMTP authentication password

```
Email__SmtpPassword=your-actual-password
```

**Recommended:** Use Azure Key Vault reference instead:
```
Email__SmtpPassword=@Microsoft.KeyVault(SecretUri=https://your-vault.vault.azure.net/secrets/SmtpPassword/)
```

---

## Optional Configuration Overrides

If you need to change company or email settings from defaults in `appsettings.json`:

### Company Settings
```
Company__Name=OCELIS SYSTEM s.r.o.
Company__Email=info@ocelis.cz
Company__Phone=+420 776 412 812
Company__Address=Františky Stránecké 165/8, Ostrava - Mariánské hory, 709 00
```

### Email Settings
```
Email__SmtpHost=wes1-smtp.wedos.net
Email__SmtpPort=587
Email__FromEmail=configurator@company.com
Email__FromName=OCELIS Konfigurátor
Email__ToEmail=david.ledvinka@post.cz
Email__EnableSsl=true
```

---

## Viewing Application Insights Data

### Azure Portal

1. Go to **Application Insights** resource
2. Navigate to **Monitoring** → **Logs**
3. Use Kusto Query Language (KQL) to query data

### Sample Queries

**Daily Inquiries:**
```kusto
customEvents
| where name == "InquirySubmitted"
| summarize count() by bin(timestamp, 1d)
| order by timestamp desc
```

**Daily Revenue Potential:**
```kusto
customMetrics
| where name == "OrderValue"
| summarize TotalValue = sum(value) by bin(timestamp, 1d)
| order by timestamp desc
```

**Recent Form Submissions:**
```kusto
customEvents
| where name == "InquirySubmitted"
| project timestamp, CustomerEmail = customDimensions.CustomerEmail,
          StavbaTyp = customDimensions.StavbaTyp,
          VaznikTyp = customDimensions.VaznikTyp
| order by timestamp desc
| take 50
```

**Email Errors:**
```kusto
traces
| where severityLevel == 3 // Error
| where message contains "email" or message contains "smtp"
| order by timestamp desc
| take 50
```

**Form Submission Errors:**
```kusto
traces
| where severityLevel == 3 // Error
| where message contains "Form submission failed"
| project timestamp, message, customDimensions.Email, customDimensions.Price
| order by timestamp desc
```

---

## Creating Dashboards

### Application Insights Workbooks

1. Go to **Application Insights** → **Workbooks**
2. Click **+ New**
3. Add tiles for:
   - Daily inquiry count (line chart)
   - Daily revenue potential (bar chart)
   - Recent submissions (table)
   - Error rate (metric)

### Power BI Integration

Application Insights data can be connected to Power BI for advanced reporting:
1. Open Power BI Desktop
2. Get Data → **Azure** → **Application Insights**
3. Enter your Application Insights ID
4. Build custom dashboards

---

## Monitoring & Alerts

### Recommended Alerts

#### Email Failure Alert
- **Condition:** When email send fails
- **Query:**
  ```kusto
  traces
  | where message contains "Email send failed"
  | summarize count()
  ```
- **Threshold:** > 0 in last 5 minutes
- **Action:** Send email to admin

#### High Error Rate Alert
- **Condition:** Error rate exceeds threshold
- **Metric:** `exceptions/count`
- **Threshold:** > 5 in last 15 minutes
- **Action:** Send email to admin

#### Form Submission Drop Alert
- **Condition:** No inquiries for extended period
- **Query:**
  ```kusto
  customEvents
  | where name == "InquirySubmitted"
  | summarize count()
  ```
- **Threshold:** = 0 in last 24 hours
- **Action:** Send email to admin

---

## Troubleshooting

### No Data in Application Insights

1. **Check connection string** is configured in Azure App Settings
2. **Verify package** `Microsoft.ApplicationInsights.AspNetCore` is installed
3. **Check logs** in Azure App Service → Log Stream
4. **Wait 5-10 minutes** for data to appear after deployment

### Email Not Sending

1. **Check SMTP credentials** in Azure App Settings
2. **Review logs** in Application Insights:
   ```kusto
   traces
   | where message contains "smtp" or message contains "email"
   | order by timestamp desc
   ```
3. **Test SMTP connection** from local environment
4. **Verify SMTP port 587** is not blocked by Azure

### Logs Not Appearing

1. **Check log levels** in `appsettings.Production.json`
2. **Verify Application Insights** is properly initialized in Program.cs
3. **Check Azure App Settings** for APPLICATIONINSIGHTS_CONNECTION_STRING
4. **Restart App Service** after configuration changes

---

## Security Best Practices

1. **Never commit** actual credentials to source control
2. **Use Azure Key Vault** for sensitive configuration (SMTP password)
3. **Restrict access** to Application Insights data
4. **Enable managed identity** for Key Vault access
5. **Rotate credentials** regularly
6. **Review logs** for suspicious activity
7. **Mask sensitive data** in logs (PII, passwords)

---

## Cost Optimization

Application Insights pricing is based on data volume:

- **Free tier:** 5 GB/month
- **Pay-as-you-go:** ~$2.88 per GB after free tier

Tips to reduce costs:
- Set appropriate **log levels** (Warning/Error in production)
- Use **sampling** for high-volume applications
- Set **data retention** to 30-90 days instead of default
- Filter out **noisy logs** (health checks, static files)

---

## Next Steps

1. ✅ Configure APPLICATIONINSIGHTS_CONNECTION_STRING in Azure
2. ✅ Configure Email__SmtpUsername and Email__SmtpPassword in Azure
3. ✅ Deploy updated application to Azure
4. ✅ Verify logs appear in Application Insights
5. ✅ Create dashboard for business metrics
6. ✅ Set up email failure alerts
7. ✅ Review and optimize log levels based on volume
