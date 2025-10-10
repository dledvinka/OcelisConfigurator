1. Dodat obsah - Obrázky - grafiku na stránku. Ideálně 1 obrázek produktu s šikmou střechou a jeden s rovnou střechou. Ideálně 800x800px.

2. Dodat Excel s výpočty. Excel jako formát není úplně vhodný. Pokud to půjde, bude to převádět do jiného formátu vhodnějšího pro použití aplikace. Načítání Excelu by mohlo aplikaci zpomalovat.
Jak často se bude Excel s výpočty měnit?

3. Maily budeme posílat přes SMTP server Ocelis?
Pokud ano, potřebuju tyto hodnoty:
{
  "Email": {
    "SmtpHost": "smtp.company.com",          // SMTP server address
    "SmtpPort": "587",                       // Port (587 for TLS, 465 for SSL, 25 for no encryption)
    "SmtpUsername": "configurator@company.com", // SMTP username (if authentication required)
    "SmtpPassword": "PASSWORD_HERE",         // SMTP password (if authentication required)
    "FromEmail": "configurator@company.com", // Sender email address
    "FromName": "Ocelis Konfigurátor",      // Sender name
    "ToEmail": "owner@company.com",          // Recipient email (company owner)
    "EnableSsl": "true"                      // Use SSL/TLS (true for secure connection)
  }
}

3. Kde se to bude hostovat?
Začněme s tímto - nejjednodušší varianta - Můžu to nahostovat v cloudu (Azure), dá se tam pak dopojit jejich doména. Výhody - je to jednoduché a můžu to spravovat vzdáleně.
Nejlevější možnost 13.14USD bez DPH / měsíc.
Je tam možnost zdarma, ale tam se aplikace uspává. První spuštění je velmi pomalé., pro nás nepoužitelné.

Pokud si to budou chtít hostovat u sebe tak jak? V případě problému nebudu schopen řešit na dálku. Nebudu schopen na dálku nasadit novou verzi.
a. Windows Server - poskytnu výstup ve formě .NET 9 webové aplikace - musí si sami nasadit na IIS. Poskytnu kdyžtak nějaké instrukce. 
b. Linux server - poskytnu instrukce - vypadá to složitě.
c. Docker - poskytnu isntrukce - taky to bude složitější asi.