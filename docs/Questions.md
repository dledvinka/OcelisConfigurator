1. Obrázky - grafiku na stránku. Ideálně 1 obrázek produktu s šikmou střechou a jeden s rovnou střechou. Ideálně 800x800px. Doplnit telefonické kontakt a email.

2. Excel s výpočty. Excel jako formát není úplně vhodný. Pokud to půjde, bude to převádět do jiného formátu. Jak často se bude Excel s výpočty měnit.

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
Začněme s tímto - Nejjednodušší varianta - Můžu to nahostovat v cloudu (Azure), dá se tam pak dopojit jejich doména. Je tam možnost zdarma - můžem to zkusit, ale může to být pomalé. Pro tento scénář by to mohlo stačit.
Výhody - je to jednoduché a můžu to sprvovat vzdáleně.
Pokud by to bylo pomalé, musíme přejít na silnější server - minimální cena (nejmenší výkon je 13.14USD bez DPH / month). 

Pokud si to budou chtít hostovat u sebe tak jak? V případě problému nebudu schopen řešit na dálku. Nebudu schopen na dálku nasadit novou verzi.
a. Windows Server - poskytnu výstup ve formě .NET 9 webové aplikace - musí si sami nasadit na IIS. Poskytnu kdyžtak nějaké instrukce. 
b. Linux server - poskytnu instrukce - vypadá to složitě.
c. Docker - poskytnu isntrukce - taky to bude složitější asi.