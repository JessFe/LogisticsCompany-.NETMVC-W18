using Spedizioni.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Spedizioni.Controllers
{
    public class ClienteController : Controller
    {
        // stringa di connessione al database
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // GET: Cliente

        // Mostra la lista dei clienti
        public ActionResult Index()
        {
            // Inizializza una lista di clienti
            List<Cliente> clienti = new List<Cliente>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query che seleziona tutti i clienti
                string sql = "SELECT * FROM Clienti";
                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiunge un nuovo cliente alla lista
                        clienti.Add(new Cliente
                        {
                            IDCliente = (int)reader["IDCliente"],
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            NomeAzienda = reader["NomeAzienda"].ToString(),
                            Indirizzo = reader["Indirizzo"].ToString(),
                            CodiceFiscale = reader["CodiceFiscale"].ToString(),
                            PartitaIVA = reader["PartitaIVA"].ToString(),
                            TipoCliente = reader["TipoCliente"].ToString(),
                            Email = reader["Email"].ToString(),
                            Telefono = reader["Telefono"].ToString()
                        });
                    }
                }
                // In caso di errore
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(clienti);
        }

        //GET: Cliente/Create
        public ActionResult Create()
        {
            // Popola la dropdownlist
            PopulateTipoClienteDropDownList();
            return View();
        }

        // POST: Cliente/Create
        [HttpPost]
        public ActionResult Create(Cliente cliente)
        {
            // Controlla se i dati inseriti sono validi
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Apre la connessione al database
                    conn.Open();
                    // Query per l'inserimento del cliente
                    string sql = "INSERT INTO Clienti (TipoCliente, Indirizzo, Email, Telefono";
                    string values = "VALUES (@TipoCliente, @Indirizzo, @Email, @Telefono";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    // Aggiunge i parametri
                    cmd.Parameters.AddWithValue("@TipoCliente", cliente.TipoCliente);
                    cmd.Parameters.AddWithValue("@Indirizzo", cliente.Indirizzo);
                    // Se l'email o il telefono sono vuoti, vengono inseriti come DBNull.Value
                    // DBNull.Value è un valore speciale che indica che il valore è nullo
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(cliente.Email) ? (object)DBNull.Value : cliente.Email);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(cliente.Telefono) ? (object)DBNull.Value : cliente.Telefono);

                    // Aggiunge i parametri in base al tipo di cliente
                    if (cliente.TipoCliente == "Privato")
                    {
                        sql += ", Nome, Cognome, CodiceFiscale";
                        values += ", @Nome, @Cognome, @CodiceFiscale";
                        cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                        cmd.Parameters.AddWithValue("@Cognome", cliente.Cognome);
                        cmd.Parameters.AddWithValue("@CodiceFiscale", cliente.CodiceFiscale);
                    }
                    else if (cliente.TipoCliente == "Azienda")
                    {
                        sql += ", NomeAzienda, PartitaIVA";
                        values += ", @NomeAzienda, @PartitaIVA";
                        cmd.Parameters.AddWithValue("@NomeAzienda", cliente.NomeAzienda);
                        cmd.Parameters.AddWithValue("@PartitaIVA", cliente.PartitaIVA);
                    }

                    // Completamento della query
                    sql += ") " + values + ")";
                    cmd.CommandText = sql;


                    try
                    {

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Non è stato possibile aggiungere il cliente.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                    }
                }
            }

            // Riprepara la dropdownlist in caso di errore
            PopulateTipoClienteDropDownList(cliente.TipoCliente);
            return View(cliente);
        }


        // Metodo per popolare la dropdownlist
        private void PopulateTipoClienteDropDownList(object selectedTipo = null)
        {
            // Crea una lista di elementi per la dropdownlist
            var listTipo = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Privato", Value = "Privato" },
                        new SelectListItem { Text = "Azienda", Value = "Azienda" }
                    };

            // Passa la lista alla viewbag per la dropdownlist
            ViewBag.TipoCliente = new SelectList(listTipo, "Value", "Text", selectedTipo);
        }
    }
}