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

        //GET: AggiornamentoSpedizione/Create
        public ActionResult Create()
        {
            // Popola la dropdownlist con le spedizioni
            PopulateSpedizioneDropDownList();

            // Popola la dropdownlist con gli stati di spedizione
            PopulateStatiDropDownList();

            return View();
        }

        //POST: AggiornamentoSpedizione/Create
        [HttpPost]
        public ActionResult Create(AggiornamentoSpedizione aggiornamento)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Apre la connessione al database
                    conn.Open();
                    // Query per inserire un nuovo aggiornamento
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

            // Ripopola la DropDownList e altre ViewBag in caso di fallimento della validazione del modello o altri errori
            PopulateSpedizioneDropDownList(aggiornamento.FK_IDSpedizione); // Riutilizza l'ID selezionato per mantenere la selezione dell'utente

            // Ricrea ViewBag.StatoSped con gli stati di spedizione
            PopulateStatiDropDownList(aggiornamento.StatoSped);

            // Ritorna alla vista con il modello corrente per mostrare gli errori o permettere all'utente di correggere l'input
            return View(aggiornamento);
        }

        // GET: AggiornamentoSpedizione/Edit/5
        public ActionResult Edit(int id)
        {
            AggiornamentoSpedizione aggiornamento = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM AggiornamentiSpedizioni WHERE IDAggiornamento = @IDAggiornamento";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IDAggiornamento", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
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
                    // Log l'errore (esempio: Console.WriteLine(ex.Message))
                }
            }

            if (aggiornamento == null)
            {
                return HttpNotFound();
            }

            PopulateSpedizioneDropDownList(aggiornamento.FK_IDSpedizione);
            PopulateStatiDropDownList(aggiornamento.StatoSped);

            return View(aggiornamento);
        }

        // POST: AggiornamentoSpedizione/Edit/5
        [HttpPost]
        public ActionResult Edit(AggiornamentoSpedizione aggiornamento)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "UPDATE AggiornamentiSpedizioni SET FK_IDSpedizione = @FK_IDSpedizione, StatoSped = @StatoSped, LuogoPacco = @LuogoPacco, DescrizEvento = @DescrizEvento, UltimoAggiornamento = @UltimoAggiornamento WHERE IDAggiornamento = @IDAggiornamento";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@IDAggiornamento", aggiornamento.IDAggiornamento);
                    cmd.Parameters.AddWithValue("@FK_IDSpedizione", aggiornamento.FK_IDSpedizione);
                    cmd.Parameters.AddWithValue("@StatoSped", aggiornamento.StatoSped);
                    cmd.Parameters.AddWithValue("@LuogoPacco", aggiornamento.LuogoPacco);
                    cmd.Parameters.AddWithValue("@DescrizEvento", aggiornamento.DescrizEvento);
                    cmd.Parameters.AddWithValue("@UltimoAggiornamento", DateTime.Now);

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



        // Metodo per popolare la dropdownlist con le spedizioni

        private void PopulateSpedizioneDropDownList(object selectedSpedizione = null)
        {
            var spedizioniConDettagli = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
SELECT s.IDSpedizione, c.IDCliente, c.Nome, c.Cognome, c.NomeAzienda 
FROM Spedizioni s
INNER JOIN Clienti c ON s.FK_IDCliente = c.IDCliente";
                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
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

            ViewBag.FK_IDSpedizioneList = new SelectList(spedizioniConDettagli, "IDSpedizione", "Descrizione", selectedSpedizione);
        }


        // Metodo per popolare la dropdownlist con gli stati di spedizione
        private void PopulateStatiDropDownList(string selectedStato = null)
        {
            var statiSpedizione = new List<SelectListItem>
    {
        new SelectListItem { Text = "In Transito", Value = "In Transito" },
        new SelectListItem { Text = "In Consegna", Value = "In Consegna" },
        new SelectListItem { Text = "Consegnato", Value = "Consegnato" },
        new SelectListItem { Text = "Non Consegnato", Value = "Non Consegnato" }
    };

            ViewBag.StatoSped = new SelectList(statiSpedizione, "Value", "Text", selectedStato);
        }

    }
}