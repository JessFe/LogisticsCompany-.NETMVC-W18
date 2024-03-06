using Spedizioni.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Spedizioni.Controllers
{
    public class SpedizioneController : Controller
    {
        // stringa di connessione al database
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // GET: Spedizione

        // Mostra la lista delle spedizioni
        public ActionResult Index()
        {
            // Inizializza una lista di spedizioni
            List<Spedizione> spedizioni = new List<Spedizione>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query che seleziona tutte le spedizioni
                string sql = "SELECT * FROM Spedizioni";
                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiunge una nuova spedizione alla lista
                        spedizioni.Add(new Spedizione
                        {
                            IDSpedizione = (int)reader["IDSpedizione"],
                            FK_IDCliente = (int)reader["FK_IDCliente"],
                            DataSpedizione = (DateTime)reader["DataSpedizione"],
                            Peso = (decimal)reader["Peso"],
                            CittaDest = reader["CittaDest"].ToString(),
                            IndirizzoDest = reader["IndirizzoDest"].ToString(),
                            NominativoDest = reader["NominativoDest"].ToString(),
                            CostoSpedizione = reader["CostoSpedizione"] as decimal?,
                            DataConsegna = reader["DataConsegna"] as DateTime?
                        });
                    }
                }
                // In caso di errore
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return View(spedizioni);
        }

        //GET: Spedizione/Create
        public ActionResult Create()
        {
            //Popola la dropdownlist con i clienti
            PopulateClienteDropDownList();
            return View();
        }

        //POST: Spedizione/Create
        [HttpPost]

        public ActionResult Create(Spedizione spedizione)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Apre la connessione al database
                    conn.Open();
                    // Query per l'inserimento della spedizione
                    string sql = "INSERT INTO Spedizioni (FK_IDCliente, DataSpedizione, Peso, CittaDest, IndirizzoDest, NominativoDest, CostoSpedizione, DataConsegna) VALUES (@FK_IDCliente, @DataSpedizione, @Peso, @CittaDest, @IndirizzoDest, @NominativoDest, @CostoSpedizione, @DataConsegna)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Connection = conn;

                    // Aggiunge i parametri della query
                    cmd.Parameters.AddWithValue("@FK_IDCliente", spedizione.FK_IDCliente);
                    cmd.Parameters.AddWithValue("@DataSpedizione", spedizione.DataSpedizione);
                    cmd.Parameters.AddWithValue("@Peso", spedizione.Peso);
                    cmd.Parameters.AddWithValue("@CittaDest", spedizione.CittaDest);
                    cmd.Parameters.AddWithValue("@IndirizzoDest", spedizione.IndirizzoDest);
                    cmd.Parameters.AddWithValue("@NominativoDest", spedizione.NominativoDest);
                    cmd.Parameters.AddWithValue("@CostoSpedizione", spedizione.CostoSpedizione);
                    cmd.Parameters.AddWithValue("@DataConsegna", spedizione.DataConsegna);


                    try
                    {

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Non è stato possibile aggiungere la spedizione.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                    }
                }
            }

            return View(spedizione);
        }

        // GET: Spedizione/Edit/5
        public ActionResult Edit(int id)
        {
            Spedizione spedizione = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Spedizioni WHERE IDSpedizione = @IDSpedizione";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IDSpedizione", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        spedizione = new Spedizione
                        {
                            IDSpedizione = (int)reader["IDSpedizione"],
                            FK_IDCliente = (int)reader["FK_IDCliente"],
                            DataSpedizione = (DateTime)reader["DataSpedizione"],
                            Peso = (decimal)reader["Peso"],
                            CittaDest = reader["CittaDest"].ToString(),
                            IndirizzoDest = reader["IndirizzoDest"].ToString(),
                            NominativoDest = reader["NominativoDest"].ToString(),
                            CostoSpedizione = (decimal?)reader["CostoSpedizione"],
                            DataConsegna = (DateTime?)reader["DataConsegna"]
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Gestione errori
                }
            }

            if (spedizione == null)
            {
                return HttpNotFound();
            }

            // Popolare eventuali dropdownlist o dati necessari per la vista
            PopulateClienteDropDownList(spedizione.FK_IDCliente);
            return View(spedizione);
        }

        // POST: Spedizione/Edit/5
        [HttpPost]
        public ActionResult Edit(Spedizione spedizione)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE Spedizioni SET FK_IDCliente = @FK_IDCliente, DataSpedizione = @DataSpedizione, Peso = @Peso, CittaDest = @CittaDest, IndirizzoDest = @IndirizzoDest, NominativoDest = @NominativoDest, CostoSpedizione = @CostoSpedizione, DataConsegna = @DataConsegna WHERE IDSpedizione = @IDSpedizione";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@IDSpedizione", spedizione.IDSpedizione);
                    cmd.Parameters.AddWithValue("@FK_IDCliente", spedizione.FK_IDCliente);
                    cmd.Parameters.AddWithValue("@DataSpedizione", spedizione.DataSpedizione);
                    cmd.Parameters.AddWithValue("@Peso", spedizione.Peso);
                    cmd.Parameters.AddWithValue("@CittaDest", spedizione.CittaDest);
                    cmd.Parameters.AddWithValue("@IndirizzoDest", spedizione.IndirizzoDest);
                    cmd.Parameters.AddWithValue("@NominativoDest", spedizione.NominativoDest);
                    cmd.Parameters.AddWithValue("@CostoSpedizione", (object)spedizione.CostoSpedizione ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DataConsegna", (object)spedizione.DataConsegna ?? DBNull.Value);

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Non è stato possibile aggiornare la spedizione.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                    }
                }
            }

            // Riprepara la dropdownlist in caso di errore
            PopulateClienteDropDownList(spedizione.FK_IDCliente);
            return View(spedizione);
        }




        //detagli spedizione
        public ActionResult Details(int id)
        {
            Spedizione spedizione = new Spedizione();
            List<AggiornamentoSpedizione> aggiornamenti = new List<AggiornamentoSpedizione>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Recupera la spedizione
                string sqlSpedizione = "SELECT * FROM Spedizioni WHERE IDSpedizione = @IDSpedizione";
                SqlCommand cmdSpedizione = new SqlCommand(sqlSpedizione, conn);
                cmdSpedizione.Parameters.AddWithValue("@IDSpedizione", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmdSpedizione.ExecuteReader();
                    if (reader.Read()) // Utilizzo if perché mi aspetto al massimo un risultato
                    {
                        spedizione.IDSpedizione = (int)reader["IDSpedizione"];
                        spedizione.FK_IDCliente = (int)reader["FK_IDCliente"];
                        spedizione.DataSpedizione = (DateTime)reader["DataSpedizione"];
                        spedizione.Peso = (decimal)reader["Peso"];
                        spedizione.CittaDest = reader["CittaDest"].ToString();
                        spedizione.IndirizzoDest = reader["IndirizzoDest"].ToString();
                        spedizione.NominativoDest = reader["NominativoDest"].ToString();
                        spedizione.CostoSpedizione = reader["CostoSpedizione"] as decimal?;
                        spedizione.DataConsegna = reader["DataConsegna"] as DateTime?;
                    }
                    reader.Close(); // Importante chiudere il reader prima di eseguire una nuova query
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // Recupera gli aggiornamenti
                string sqlAggiornamenti = "SELECT * FROM AggiornamentiSpedizioni WHERE FK_IDSpedizione = @FK_IDSpedizione";
                SqlCommand cmdAggiornamenti = new SqlCommand(sqlAggiornamenti, conn);
                cmdAggiornamenti.Parameters.AddWithValue("@FK_IDSpedizione", id);

                try
                {
                    SqlDataReader reader = cmdAggiornamenti.ExecuteReader();
                    while (reader.Read())
                    {
                        aggiornamenti.Add(new AggiornamentoSpedizione
                        {
                            IDAggiornamento = (int)reader["IDAggiornamento"],
                            FK_IDSpedizione = (int)reader["FK_IDSpedizione"],
                            StatoSped = reader["StatoSped"].ToString(),
                            LuogoPacco = reader["LuogoPacco"].ToString(),
                            DescrizEvento = reader["DescrizEvento"].ToString(),
                            UltimoAggiornamento = (DateTime)reader["UltimoAggiornamento"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            // Prepara e restituisce il modello alla vista
            var model = new Tuple<Spedizione, List<AggiornamentoSpedizione>>(spedizione, aggiornamenti);
            return View(model);
        }


        // Metodo per popolare la dropdownlist con i clienti
        private void PopulateClienteDropDownList(object selectedCliente = null)
        {
            // Inizializza una lista di clienti
            List<Cliente> clienti = new List<Cliente>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare tutti i clienti
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
                            TipoCliente = reader["TipoCliente"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            CodiceFiscale = reader["CodiceFiscale"].ToString(),
                            NomeAzienda = reader["NomeAzienda"].ToString(),
                            PartitaIVA = reader["PartitaIVA"].ToString(),
                            Indirizzo = reader["Indirizzo"].ToString(),
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
            // Imposta la dropdownlist
            var clienteSelectList = clienti.Select(c => new
            {
                IDCliente = c.IDCliente,
                Descrizione = $"{c.IDCliente} - " + (!string.IsNullOrWhiteSpace(c.Cognome) && !string.IsNullOrWhiteSpace(c.Nome) ? $"{c.Cognome} {c.Nome}" : c.NomeAzienda)
            }).ToList();

            ViewBag.FK_IDCliente = new SelectList(clienteSelectList, "IDCliente", "Descrizione", selectedCliente);
        }
    }
}