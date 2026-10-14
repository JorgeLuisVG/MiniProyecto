using System;
using System.Collections.Generic;

class Usuario
{
    public string ID;
    public string Nombre;
    public string Rol;

    public Usuario(string id, string nombre, string rol)
    {
        ID = id;
        Nombre = nombre;
        Rol = rol;
    }
}

class Amistad 
{
    public string DestinoID;

    public Amistad(string destinoID)
    {
        DestinoID = destinoID;
    }
}

class RedSocial
{
    private Dictionary<string, Usuario> usuarios;
    private Dictionary<string, List<Amistad>> listaAdyacencia;

    public RedSocial()
    {
        usuarios = new Dictionary<string, Usuario>(StringComparer.OrdinalIgnoreCase);
        listaAdyacencia = new Dictionary<string, List<Amistad>>(StringComparer.OrdinalIgnoreCase);
    }

    public bool ExisteUsuario(string id)
    {
        return usuarios.ContainsKey(id.Trim());
    }

    public void AgregarUsuario(string id, string nombre, string rol)
    {
        id = id.Trim();
        nombre = nombre.Trim();
        rol = rol.Trim();

        if (id == "" || nombre == "")
        {
            Console.WriteLine("El ID y el nombre no pueden estar vacíos.");
            return;
        }

        if (rol == "")
        {
            rol = "Estudiante";
        }

        if (usuarios.ContainsKey(id))
        {
            Console.WriteLine("El usuario con ID '" + id + "' ya existe.");
            return;
        }

        usuarios[id] = new Usuario(id, nombre, rol);
        listaAdyacencia[id] = new List<Amistad>();
        Console.WriteLine("Usuario '" + nombre + "' (" + rol + ") agregado.");
    }

    public void AgregarAmistad(string id1, string id2)
    {
        id1 = id1.Trim();
        id2 = id2.Trim();

        if (!usuarios.ContainsKey(id1) || !usuarios.ContainsKey(id2))
        {
            Console.WriteLine("Ambos usuarios deben existir antes de crear la amistad.");
            return;
        }

        if (id1.Equals(id2, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Un usuario no puede ser amigo de sí mismo.");
            return;
        }

        if (ExisteAmistad(id1, id2))
        {
            Console.WriteLine("La amistad " + usuarios[id1].Nombre + " <-> " + usuarios[id2].Nombre + " ya existe.");
            return;
        }

        listaAdyacencia[id1].Add(new Amistad(usuarios[id2].ID));
        listaAdyacencia[id2].Add(new Amistad(usuarios[id1].ID));

        Console.WriteLine("Amistad " + usuarios[id1].Nombre + " <-> " + usuarios[id2].Nombre + " agregada.");
    }

    private bool ExisteAmistad(string id1, string id2)
    {
        foreach (Amistad arista in listaAdyacencia[id1])
        {
            if (arista.DestinoID.Equals(id2, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private List<Amistad> VecinosOrdenados(string id)
    {
        List<Amistad> copia = new List<Amistad>(listaAdyacencia[id]);
        copia.Sort((a, b) => string.Compare(usuarios[a.DestinoID].Nombre, usuarios[b.DestinoID].Nombre, StringComparison.OrdinalIgnoreCase));
        return copia;
    }

    private string Etiqueta(string id)
    {
        return usuarios[id].Nombre + " [" + usuarios[id].Rol + "]";
    }

    public void MostrarListaAdyacencia()
    {
        Console.WriteLine("Lista de adyacencia (grafo no dirigido y no ponderado)");

        List<string> ids = new List<string>(listaAdyacencia.Keys);
        ids.Sort((a, b) => string.Compare(usuarios[a].Nombre, usuarios[b].Nombre, StringComparison.OrdinalIgnoreCase));

        foreach (string id in ids)
        {
            List<Amistad> amigos = VecinosOrdenados(id);

            if (amigos.Count == 0)
            {
                Console.WriteLine(Etiqueta(id) + " (ID " + id + ") -> (sin amigos)");
                continue;
            }

            string texto = "";
            for (int i = 0; i < amigos.Count; i++)
            {
                texto += usuarios[amigos[i].DestinoID].Nombre;
                if (i < amigos.Count - 1)
                {
                    texto += ", ";
                }
            }
            Console.WriteLine(Etiqueta(id) + " (ID " + id + ") -> " + texto);
        }
    }

    public void ConexionBidireccional(string origen, string destino)
    {
        origen = usuarios[origen.Trim()].ID;
        destino = usuarios[destino.Trim()].ID;

        if (origen == destino)
        {
            Console.WriteLine("Origen y destino son la misma persona.");
            return;
        }

        Queue<string> colaA = new Queue<string>();
        Queue<string> colaB = new Queue<string>();
        Dictionary<string, int> distA = new Dictionary<string, int>();
        Dictionary<string, int> distB = new Dictionary<string, int>();
        Dictionary<string, string> previoA = new Dictionary<string, string>();
        Dictionary<string, string> previoB = new Dictionary<string, string>();

        colaA.Enqueue(origen);
        distA[origen] = 0;
        colaB.Enqueue(destino);
        distB[destino] = 0;

        int mejorTotal = int.MaxValue;
        string encuentro = null;

        Console.WriteLine("Algoritmo BFS bidireccional entre " + usuarios[origen].Nombre + " y " + usuarios[destino].Nombre + ": ");

        while (colaA.Count > 0 && colaB.Count > 0 && encuentro == null)
        {
            if (colaA.Count <= colaB.Count)
            {
                ExpandirNivel("A", colaA, distA, previoA, distB, ref mejorTotal, ref encuentro);
            }
            else
            {
                ExpandirNivel("B", colaB, distB, previoB, distA, ref mejorTotal, ref encuentro);
            }
        }

        if (encuentro == null)
        {
            Console.WriteLine("No hay conexión entre " + usuarios[origen].Nombre + " y " + usuarios[destino].Nombre + ".");
            return;
        }

        Console.WriteLine("  Las fronteras se intersectan en: " + usuarios[encuentro].Nombre);

        List<string> camino = new List<string>();
        string actual = encuentro;
        while (actual != origen)
        {
            camino.Insert(0, actual);
            actual = previoA[actual];
        }
        camino.Insert(0, origen);

        actual = encuentro;
        while (actual != destino)
        {
            actual = previoB[actual];
            camino.Add(actual);
        }

        List<string> nombres = new List<string>();
        foreach (string id in camino)
        {
            nombres.Add(usuarios[id].Nombre);
        }

        int saltos = camino.Count - 1;
        Console.WriteLine("Cadena de contactos más corta: " + string.Join(" -> ", nombres));

        if (saltos == 1)
        {
            Console.WriteLine("Son amigos directos.");
        }
        else
        {
            Console.WriteLine("Estás a " + (saltos - 1) + " contacto(s) intermedio(s) (" + saltos + " grados de separación).");
        }
    }

    private void ExpandirNivel(string lado, Queue<string> cola, Dictionary<string, int> distPropia, Dictionary<string, string> previoPropio, Dictionary<string, int> distOtra, ref int mejorTotal, ref string encuentro)
    {
        int cantidadNivel = cola.Count;
        List<string> descubiertos = new List<string>();

        for (int i = 0; i < cantidadNivel; i++)
        {
            string actual = cola.Dequeue();

            foreach (Amistad arista in VecinosOrdenados(actual))
            {
                string vecino = arista.DestinoID;

                if (distPropia.ContainsKey(vecino))
                {
                    continue;
                }

                distPropia[vecino] = distPropia[actual] + 1;
                previoPropio[vecino] = actual;
                cola.Enqueue(vecino);
                descubiertos.Add(usuarios[vecino].Nombre);

                if (distOtra.ContainsKey(vecino))
                {
                    int total = distPropia[vecino] + distOtra[vecino];
                    if (total < mejorTotal)
                    {
                        mejorTotal = total;
                        encuentro = vecino;
                    }
                }
            }
        }

        string lista = descubiertos.Count == 0 ? "nadie nuevo" : string.Join(", ", descubiertos);
        Console.WriteLine("  Frontera " + lado + " explorar nivel y descubrir: " + lista);
    }

    public void RecorridoDFS(string origen)
    {
        origen = usuarios[origen.Trim()].ID;

        HashSet<string> visitados = new HashSet<string>();
        List<string> ordenVisita = new List<string>();

        DFSRecursivo(origen, visitados, ordenVisita);

        Console.WriteLine("Algoritmo DFS desde " + usuarios[origen].Nombre + ": ");
        Console.WriteLine(string.Join(" -> ", ordenVisita));
        Console.WriteLine("Usuarios alcanzables: " + ordenVisita.Count + " de " + usuarios.Count);
    }

    private void DFSRecursivo(string actual, HashSet<string> visitados, List<string> ordenVisita)
    {
        visitados.Add(actual);
        ordenVisita.Add(usuarios[actual].Nombre);

        foreach (Amistad arista in VecinosOrdenados(actual))
        {
            if (!visitados.Contains(arista.DestinoID))
            {
                DFSRecursivo(arista.DestinoID, visitados, ordenVisita);
            }
        }
    }

    public void MostrarUsuarios()
    {
        Console.WriteLine("Usuarios registrados:");
        foreach (Usuario u in usuarios.Values)
        {
            Console.WriteLine("  " + u.ID + " - " + u.Nombre + " (" + u.Rol + ")");
        }
    }
}

class Program
{
    static void CargarDatosEjemplo(RedSocial red)
    {
        red.AgregarUsuario("1529924", "David", "Estudiante");
        red.AgregarUsuario("1523624", "Jorge", "Estudiante");
        red.AgregarUsuario("1500001", "Ana", "Estudiante");
        red.AgregarUsuario("1500002", "Luis", "Estudiante");
        red.AgregarUsuario("1500003", "Maria", "Estudiante");
        red.AgregarUsuario("1500004", "Carlos", "Estudiante");
        red.AgregarUsuario("P001", "Ing. Lopez", "Profesor");
        red.AgregarUsuario("P002", "Licda. Perez", "Profesor");
        red.AgregarUsuario("1500005", "Sofia", "Estudiante"); 

        red.AgregarAmistad("1529924", "1500001");
        red.AgregarAmistad("1529924", "1500002");
        red.AgregarAmistad("1500001", "1500003");
        red.AgregarAmistad("1500002", "1500003");
        red.AgregarAmistad("1500003", "1500004");
        red.AgregarAmistad("1500004", "P001");
        red.AgregarAmistad("P001", "P002");
        red.AgregarAmistad("1523624", "P002");
    }

    static string PedirDato(string mensaje)
    {
        Console.Write(mensaje);
        return (Console.ReadLine() ?? "").Trim();
    }

    static void Main()
    {
        RedSocial red = new RedSocial();
        CargarDatosEjemplo(red);

        int opcion = 0;

        while (opcion != 7)
        {
            Console.WriteLine();
            Console.WriteLine("RED SOCIAL UNIVERSITARIA");
            Console.WriteLine("1. Mostrar lista de adyacencia");
            Console.WriteLine("2. Mostrar usuarios");
            Console.WriteLine("3. BFS bidireccional");
            Console.WriteLine("4. DFS de usuarios alcanzables");
            Console.WriteLine("5. Agregar usuario");
            Console.WriteLine("6. Agregar amistad");
            Console.WriteLine("7. Salir");

            if (!int.TryParse(PedirDato("Seleccione una opción: "), out opcion))
            {
                opcion = 0;
            }

            switch (opcion)
            {
                case 1:
                    red.MostrarListaAdyacencia();
                    break;

                case 2:
                    red.MostrarUsuarios();
                    break;

                case 3:
                    string origenBfs = PedirDato("ID del usuario de origen: ");
                    string destinoBfs = PedirDato("ID del usuario de destino: ");
                    if (red.ExisteUsuario(origenBfs) && red.ExisteUsuario(destinoBfs))
                    {
                        red.ConexionBidireccional(origenBfs, destinoBfs);
                    }
                    else
                    {
                        Console.WriteLine("Origen o destino no existe.");
                    }
                    break;

                case 4:
                    string origenDfs = PedirDato("ID del usuario de origen: ");
                    if (red.ExisteUsuario(origenDfs))
                    {
                        red.RecorridoDFS(origenDfs);
                    }
                    else
                    {
                        Console.WriteLine("El usuario no existe.");
                    }
                    break;

                case 5:
                    string id = PedirDato("ID: ");
                    string nombre = PedirDato("Nombre: ");
                    string rol = PedirDato("Rol (Estudiante/Profesor): ");
                    red.AgregarUsuario(id, nombre, rol);
                    break;

                case 6:
                    string id1 = PedirDato("ID del primer usuario: ");
                    string id2 = PedirDato("ID del segundo usuario: ");
                    red.AgregarAmistad(id1, id2);
                    break;

                case 7:
                    Console.WriteLine("Saliendo...");
                    break;

                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }
}