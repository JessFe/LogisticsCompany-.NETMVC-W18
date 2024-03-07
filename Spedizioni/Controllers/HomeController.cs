using Spedizioni.Models;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Security;

namespace Spedizioni.Controllers
{
    public class HomeController : Controller
    {
        // stringa di connessione al database
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }

        // --- RICERCA SPEDIZIONE ---

        public ActionResult VerificaSpedizione()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerificaSpedizione(VerificaSpedizione model)
        {
            if (ModelState.IsValid)
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Query per verificare se il Codice Fiscale/Partita IVA e IDSpedizione corrispondono
                    string sql = @"
SELECT COUNT(*) 
FROM Spedizioni s
JOIN Clienti c ON s.FK_IDCliente = c.IDCliente
WHERE (c.CodiceFiscale = @CodiceFiscalePartitaIVA OR c.PartitaIVA = @CodiceFiscalePartitaIVA)
AND s.IDSpedizione = @IDSpedizione";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@CodiceFiscalePartitaIVA", model.CodiceFiscalePartitaIVA);
                    cmd.Parameters.AddWithValue("@IDSpedizione", model.IDSpedizione);

                    conn.Open();
                    int exists = (int)cmd.ExecuteScalar();
                    // Se esiste una corrispondenza, reindirizza alla pagina di dettaglio spedizione
                    if (exists > 0)
                    {
                        return RedirectToAction("Details", "Spedizione", new { id = model.IDSpedizione });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Attenzione! Nessuna spedizione trovata.");
                        ModelState.AddModelError("", "Prova a ricontrollare i dati inseriti");
                    }
                }
            }


            return View(model);
        }

        // --- LOGIN ---

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login u, string returnUrl)
        {

            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                // Query per verificare se esiste un utente con email e password inseriti
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clienti WHERE Email = @Email AND Password = @Password", conn);
                cmd.Parameters.AddWithValue("Email", u.Email);
                cmd.Parameters.AddWithValue("Password", u.Password);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    FormsAuthentication.SetAuthCookie(u.Email, false);

                    // Controlla se l'URL di ritorno non è vuoto e appartiene all'applicazione
                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.AuthError = "Autenticazione non riuscita";
                    return View(u);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }

            return View();


        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
