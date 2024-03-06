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

        // GET: Cliente/Edit/5
        public ActionResult Edit(int id)
        {
            Cliente cliente = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Clienti WHERE IDCliente = @IDCliente";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IDCliente", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        cliente = new Cliente
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
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Gestire l'errore (ad esempio, loggare l'errore e reindirizzare a una pagina di errore)
                }
            }

            if (cliente == null)
            {
                // Se non trova il cliente, reindirizza ad esempio alla lista dei clienti con un messaggio
                return RedirectToAction("Index");
            }

            PopulateTipoClienteDropDownList(cliente.TipoCliente);
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        [HttpPost]
        public ActionResult Edit(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Inizia a costruire la query SQL per l'aggiornamento
                    string sql = "UPDATE Clienti SET TipoCliente = @TipoCliente, Indirizzo = @Indirizzo, Email = @Email, Telefono = @Telefono";

                    // Aggiungi i parametri per i campi comuni
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@TipoCliente", cliente.TipoCliente);
                    cmd.Parameters.AddWithValue("@Indirizzo", cliente.Indirizzo);
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(cliente.Email) ? DBNull.Value : (object)cliente.Email);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(cliente.Telefono) ? DBNull.Value : (object)cliente.Telefono);

                    // Aggiungi i parametri in base al tipo di cliente e estendi la query SQL
                    if (cliente.TipoCliente == "Privato")
                    {
                        sql += ", Nome = @Nome, Cognome = @Cognome, CodiceFiscale = @CodiceFiscale";
                        cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                        cmd.Parameters.AddWithValue("@Cognome", cliente.Cognome);
                        cmd.Parameters.AddWithValue("@CodiceFiscale", cliente.CodiceFiscale);
                    }
                    else if (cliente.TipoCliente == "Azienda")
                    {
                        sql += ", NomeAzienda = @NomeAzienda, PartitaIVA = @PartitaIVA";
                        cmd.Parameters.AddWithValue("@NomeAzienda", cliente.NomeAzienda);
                        cmd.Parameters.AddWithValue("@PartitaIVA", cliente.PartitaIVA);
                    }

                    // Completa la query con la clausola WHERE
                    sql += " WHERE IDCliente = @IDCliente";
                    cmd.Parameters.AddWithValue("@IDCliente", cliente.IDCliente);

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
                            ModelState.AddModelError("", "Non è stato possibile aggiornare il cliente.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                    }
                }
            }

            PopulateTipoClienteDropDownList(cliente.TipoCliente);
            return View(cliente);
        }

        // GET: Cliente/Dettagli/5
        public ActionResult Details(int id)
        {
            Cliente cliente = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Preparare la query SQL per selezionare il cliente per ID
                string sql = "SELECT * FROM Clienti WHERE IDCliente = @IDCliente";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IDCliente", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Se trova il cliente, crea un nuovo oggetto Cliente e assegna i valori
                    if (reader.Read())
                    {
                        cliente = new Cliente
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
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }

            if (cliente == null)
            {
                return RedirectToAction("Index");
            }

            // Se trova il cliente, passa il modello alla vista dei dettagli
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