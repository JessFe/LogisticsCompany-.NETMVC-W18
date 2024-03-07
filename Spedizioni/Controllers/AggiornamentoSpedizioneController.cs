using Spedizioni.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Spedizioni.Controllers
{

    public class AggiornamentoSpedizioneController : Controller
    {
        // stringa di connessione al database
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // GET: AggiornamentoSped

        // Mostra la lista degli aggiornamenti delle spedizioni
        public ActionResult Index()
        {
            // Inizializza la lista degli aggiornamenti
            List<AggiornamentoSpedizione> aggiornamenti = new List<AggiornamentoSpedizione>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare tutti gli aggiornamenti delle spedizioni
                string sql = "SELECT * FROM AggiornamentiSpedizioni";

                // Crea un nuovo comando per eseguire la query
                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    // Apre la connessione al database 
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiunge alla lista degli aggiornamenti un nuovo oggetto AggiornamentoSpedizione
                        aggiornamenti.Add(new AggiornamentoSpedizione
                        {
                            IDAggiornamento = (int)reader["IDAggiornamento"],
                            FK_IDSpedizione = (int)reader["FK_IDSpedizione"],
                            StatoSped = reader["StatoSped"].ToString(),
                            LuogoPacco = reader["LuogoPacco"].ToString(),
                            DescrizEvento = reader["DescrizEvento"].ToString(),
                            UltimoAggiornamento = Convert.ToDateTime(reader["UltimoAggiornamento"])
                        });
                    }
                }
                // In caso di errore
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            return View(aggiornamenti);
        }

        // --- CREATE ---

        //GET: AggiornamentoSpedizione/Create
        public ActionResult Create(int? idSpedizione = null) // Aggiungi un parametro opzionale
        {
            // Popola la dropdownlist con le spedizioni
            PopulateSpedizioneDropDownList(idSpedizione); // Modifica questa chiamata

            // Popola la dropdownlist con gli stati di spedizione
            PopulateStatiDropDownList();

            // Se idSpedizione non è null, impostalo nel modello 
            if (idSpedizione.HasValue)
            {
                var aggiornamento = new AggiornamentoSpedizione
                {
                    FK_IDSpedizione = idSpedizione.Value
                };
                return View(aggiornamento);
            }

            return View();
        }


        //POST: AggiornamentoSpedizione/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AggiornamentoSpedizione aggiornamento)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Apre la connessione al database
                    conn.Open();
                    // Query per inserire un nuovo aggiornamento spedizione
                    string sql = "INSERT INTO AggiornamentiSpedizioni (FK_IDSpedizione, StatoSped, LuogoPacco, DescrizEvento, UltimoAggiornamento) VALUES (@FK_IDSpedizione, @StatoSped, @LuogoPacco, @DescrizEvento, @UltimoAggiornamento)";
                    // Crea un nuovo comando per eseguire la query
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Connection = conn;

                    // Aggiunge i parametri alla query
                    cmd.Parameters.AddWithValue("@FK_IDSpedizione", aggiornamento.FK_IDSpedizione);
                    cmd.Parameters.AddWithValue("@StatoSped", aggiornamento.StatoSped);
                    cmd.Parameters.AddWithValue("@LuogoPacco", aggiornamento.LuogoPacco);
                    cmd.Parameters.AddWithValue("@DescrizEvento", aggiornamento.DescrizEvento);
                    cmd.Parameters.AddWithValue("@UltimoAggiornamento", DateTime.Now);


                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Non è stato possibile aggiungere l'aggiornamento.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                    }
                }
            }

            // Ripopola la DropDownList in caso di fallimento validazione modello o altri errori
            PopulateSpedizioneDropDownList(aggiornamento.FK_IDSpedizione);

            PopulateStatiDropDownList(aggiornamento.StatoSped);

            // Ritorna alla vista con il modello corrente per mostrare gli errori o permettere all'utente di correggere l'input
            return View(aggiornamento);
        }

        // --- EDIT ---

        // GET: AggiornamentoSpedizione/Edit/5
        public ActionResult Edit(int id)
        {
            // Inizializza un oggetto AggiornamentoSpedizione
            AggiornamentoSpedizione aggiornamento = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare l'aggiornamento della spedizione con l'id specificato
                string sql = "SELECT * FROM AggiornamentiSpedizioni WHERE IDAggiornamento = @IDAggiornamento";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IDAggiornamento", id);

                try
                {
                    // Apre la connessione al database
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Inizializza l'oggetto AggiornamentoSpedizione con i dati del database
                        aggiornamento = new AggiornamentoSpedizione
                        {
                            IDAggiornamento = (int)reader["IDAggiornamento"],
                            FK_IDSpedizione = (int)reader["FK_IDSpedizione"],
                            StatoSped = reader["StatoSped"].ToString(),
                            LuogoPacco = reader["LuogoPacco"].ToString(),
                            DescrizEvento = reader["DescrizEvento"].ToString(),
                            UltimoAggiornamento = (DateTime)reader["UltimoAggiornamento"]
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            // Se l'aggiornamento non è stato trovato, ritorna HttpNotFound
            if (aggiornamento == null)
            {
                return HttpNotFound();
            }

            // Popola la dropdownlist con le spedizioni
            PopulateSpedizioneDropDownList(aggiornamento.FK_IDSpedizione);
            PopulateStatiDropDownList(aggiornamento.StatoSped);

            return View(aggiornamento);
        }

        // POST: AggiornamentoSpedizione/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AggiornamentoSpedizione aggiornamento)
        {
            // Se il modello è valido
            if (ModelState.IsValid)
            {
                // Crea una nuova connessione al database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Query per aggiornare l'aggiornamento della spedizione
                    string sql = "UPDATE AggiornamentiSpedizioni SET FK_IDSpedizione = @FK_IDSpedizione, StatoSped = @StatoSped, LuogoPacco = @LuogoPacco, DescrizEvento = @DescrizEvento, UltimoAggiornamento = @UltimoAggiornamento WHERE IDAggiornamento = @IDAggiornamento";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    // Aggiunge i parametri alla query
                    cmd.Parameters.AddWithValue("@IDAggiornamento", aggiornamento.IDAggiornamento);
                    cmd.Parameters.AddWithValue("@FK_IDSpedizione", aggiornamento.FK_IDSpedizione);
                    cmd.Parameters.AddWithValue("@StatoSped", aggiornamento.StatoSped);
                    cmd.Parameters.AddWithValue("@LuogoPacco", aggiornamento.LuogoPacco);
                    cmd.Parameters.AddWithValue("@DescrizEvento", aggiornamento.DescrizEvento);
                    cmd.Parameters.AddWithValue("@UltimoAggiornamento", DateTime.Now);

                    // Tenta di eseguire la query
                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Non è stato possibile aggiornare l'aggiornamento della spedizione.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                    }
                }
            }

            // Ri-popola la DropDownList 
            PopulateSpedizioneDropDownList(aggiornamento.FK_IDSpedizione);
            PopulateStatiDropDownList(aggiornamento.StatoSped);


            return View(aggiornamento);
        }


        // --- METODI ---

        // METODO per popolare la dropdownlist con le spedizioni

        private void PopulateSpedizioneDropDownList(object selectedSpedizione = null)
        {
            // Inizializza la lista delle spedizioni
            var spedizioniConDettagli = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare tutte le spedizioni
                // INNER JOIN per ottenere i dettagli del cliente
                string sql = @"
SELECT s.IDSpedizione, c.IDCliente, c.Nome, c.Cognome, c.NomeAzienda 
FROM Spedizioni s
INNER JOIN Clienti c ON s.FK_IDCliente = c.IDCliente";
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Tenta di eseguire la query
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    // Aggiunge alla lista delle spedizioni un nuovo oggetto con IDSpedizione e Descrizione
                    while (reader.Read())
                    {
                        var nomeCompleto = $"{reader["Nome"]} {reader["Cognome"]}".Trim();
                        var descrizione = string.IsNullOrWhiteSpace(nomeCompleto) ? reader["NomeAzienda"].ToString() : nomeCompleto;

                        spedizioniConDettagli.Add(new
                        {
                            IDSpedizione = (int)reader["IDSpedizione"],
                            Descrizione = $"Sped: #{reader["IDSpedizione"]} - Mittente: {(int)reader["IDCliente"]} {descrizione}"
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Imposta la dropdownlist con le spedizioni
            ViewBag.FK_IDSpedizioneList = new SelectList(spedizioniConDettagli, "IDSpedizione", "Descrizione", selectedSpedizione);
        }


        // METODO per popolare la dropdownlist con gli stati di spedizione
        private void PopulateStatiDropDownList(string selectedStato = null)
        {
            // Inizializza la lista degli stati di spedizione
            var statiSpedizione = new List<SelectListItem>

            // Aggiunge alla lista degli stati di spedizione
    {
        new SelectListItem { Text = "In Transito", Value = "In Transito" },
        new SelectListItem { Text = "In Consegna", Value = "In Consegna" },
        new SelectListItem { Text = "Consegnato", Value = "Consegnato" },
        new SelectListItem { Text = "Non Consegnato", Value = "Non Consegnato" }
    };
            // Imposta la dropdownlist con gli stati di spedizione
            ViewBag.StatoSped = new SelectList(statiSpedizione, "Value", "Text", selectedStato);
        }

    }
}