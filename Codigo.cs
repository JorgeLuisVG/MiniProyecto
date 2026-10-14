public class Usuario // usuario
{
    public string ID { get; set; }
    public string Nombre { get; set; }

    public Usuario(string id, string nombre)
    {
        ID = id;
        Nombre = nombre;
    }
}

public class Amistad // Arista
{
    public string DestinoID { get; set; }

    public Amistad(string destinoID)
    {
        DestinoID = destinoID;
    }
}

public class RedSocial // Operacion de la red social
{
    private Dictionary<string, Usuario> usuarios;
    private Dictionary<string, List<AristaAmistad>> listaAdyacencia;

    public RedSocial()
    {
        usuarios = new Dictionary<string, Usuario>();
        listaAdyacencia = new Dictionary<string, List<AristaAmistad>>();
    }

    public bool ExisteUsuario(string id)
    {
        return usuarios.ContainsKey(id);
    }

    public bool AgregarUsuario(string id, string nombre)
    {
        id = id;
        nombre = nombre;

        if (id == "" || nombre == "")
            return false;

        if (usuarios.ContainsKey(id))
            return false;

        Usuario nuevo = new Usuario(id, nombre);

        usuarios[id] = nuevo;
        listaAdyacencia[id] = new List<AristaAmistad>();

        return true;
    }

    public Usuario ObtenerUsuario(string id)
    {
        if (usuarios.ContainsKey(id))
            return usuarios[id];

        return null;
    }

    public bool AgregarAmistad(string id1, string id2)
    {
        id1 = id1;
        id2 = id2;

        if (!usuarios.ContainsKey(id1) ||
            !usuarios.ContainsKey(id2))
            return false;

        if (id1 == id2)
            return false;

        if (ExisteAmistad(id1, id2))
            return false;

        listaAdyacencia[id1].Add(new AristaAmistad(id2));
        listaAdyacencia[id2].Add(new AristaAmistad(id1));

        return true;
    }

    public bool ExisteAmistad(string id1, string id2)
    {
        if (!listaAdyacencia.ContainsKey(id1) ||
            !listaAdyacencia.ContainsKey(id2))
            return false;

        foreach (AristaAmistad arista in listaAdyacencia[id1])
        {
            if (arista.DestinoID == id2)
                return true;
        }

        return false;
    }

    public List<Usuario> ObtenerAmigos(string id)
    {
        List<Usuario> amigos = new List<Usuario>();

        if (!listaAdyacencia.ContainsKey(id))
            return amigos;

        foreach (AristaAmistad arista in listaAdyacencia[id])
        {
            amigos.Add(usuarios[arista.DestinoID]);
        }

        return amigos;
    }

    public void MostrarRed()
    {
        foreach (string id in listaAdyacencia.Keys)
        {
            Console.Write(usuarios[id].Nombre + " -> ");

            List<string> nombresAmigos = new List<string>();

            foreach (AristaAmistad arista in listaAdyacencia[id])
            {
                nombresAmigos.Add(usuarios[arista.DestinoID].Nombre);
            }

            Console.WriteLine(string.Join(", ", nombresAmigos));
        }
    }

    public List<Usuario> BuscarCaminoBFS(string origen, string destino)
    {
        List<Usuario> camino = new List<Usuario>();

        if (!ExisteUsuario(origen) || !ExisteUsuario(destino))
            return camino;

        Queue<string> cola = new Queue<string>();
        HashSet<string> visitados = new HashSet<string>();
        Dictionary<string, string> previo = new Dictionary<string, string>();

        cola.Enqueue(origen);
        visitados.Add(origen);

        while (cola.Count > 0)
        {
            string actual = cola.Dequeue();

            if (actual == destino)
                break;

            foreach (AristaAmistad arista in listaAdyacencia[actual])
            {
                string vecino = arista.DestinoID;

                if (!visitados.Contains(vecino))
                {
                    visitados.Add(vecino);
                    previo[vecino] = actual;
                    cola.Enqueue(vecino);
                }
            }
        }

        if (!visitados.Contains(destino))
            return camino;

        string paso = destino;
        camino.Insert(0, usuarios[paso]);

        while (paso != origen)
        {
            paso = previo[paso];
            camino.Insert(0, usuarios[paso]);
        }

        return camino;
    }

    public List<Usuario> BuscarConexionesDFS(string origen)
    {
        List<Usuario> resultado = new List<Usuario>();

        if (!ExisteUsuario(origen))
            return resultado;

        HashSet<string> visitados = new HashSet<string>();

        DFSRecursivo(origen, visitados, resultado);

        return resultado;
    }

    private void DFSRecursivo(
        string actual,
        HashSet<string> visitados,
        List<Usuario> resultado)
    {
        visitados.Add(actual);
        resultado.Add(usuarios[actual]);

        foreach (AristaAmistad arista in listaAdyacencia[actual])
        {
            string vecino = arista.DestinoID;

            if (!visitados.Contains(vecino))
            {
                DFSRecursivo(vecino, visitados, resultado);
            }
        }
    }

    public int CantidadUsuarios()
    {
        return usuarios.Count;
    }

    public int CantidadAmigos(string id)
    {
        if (!listaAdyacencia.ContainsKey(id))
            return 0;

        return listaAdyacencia[id].Count;
    }
}

static class Programa // Poner aqui todo lo del uso del programa
{
    static int TryCatchInt(List<int> ListaOpciones, string Texto)
    {
        while (true)
        {
            int Ingresar;

            while (true)
            {
                try
                {
                    Console.WriteLine(Texto);
                    Ingresar = int.Parse(Console.ReadLine().Trim());
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Ingrese un valor válido");
                }
            }

            if (ListaOpciones.Count > 0)
            {
                if (ListaOpciones.Contains(Ingresar))
                {
                    return Ingresar;
                }
                else
                {
                    Console.WriteLine("Inténtelo nuevamente");
                }
            }
            else
            {
                return Ingresar;
            }
        }
    }

    static string TryCatchString(string Texto, List<string> Lista)
    {
        while (true)
        {
            string Ingresar;
            while (true)
            {
                try
                {
                    Console.WriteLine(Texto);
                    Ingresar = Console.ReadLine().Trim().ToUpper();
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Intetelo Nuevamente");
                }
            }
            if (Ingresar == "")
            {
                Console.WriteLine("Ingrese un Texto valido");
                continue;
            }

            if (Lista.Count > 0)
            {
                if (Lista.Contains(Ingresar))
                {
                    return Ingresar;
                }
            }
            else { return Ingresar; }

        }

    }


}