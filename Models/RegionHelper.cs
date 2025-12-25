namespace TechnicianAgenda.Models
{
    public static class RegionHelper
    {
        public static Dictionary<Region, string> RegionNames = new()
        {
            { Region.AricaYParinacota, "Región de Arica y Parinacota" },
            { Region.Tarapaca, "Región de Tarapacá" },
            { Region.Antofagasta, "Región de Antofagasta" },
            { Region.Atacama, "Región de Atacama" },
            { Region.Coquimbo, "Región de Coquimbo" },
            { Region.Valparaiso, "Región de Valparaíso" },
            { Region.RegionMetropolitana, "Región Metropolitana de Santiago" },
            { Region.LiberatorOHiggins, "Región del Libertador General Bernardo O'Higgins" },
            { Region.Maule, "Región del Maule" },
            { Region.Nuble, "Región de Ñuble" },
            { Region.Biobio, "Región del Biobío" },
            { Region.LaAraucania, "Región de La Araucanía" },
            { Region.LosRios, "Región de Los Ríos" },
            { Region.LosLagos, "Región de Los Lagos" },
            { Region.Aysen, "Región de Aysén del General Carlos Ibáñez del Campo" },
            { Region.MagallanesYAntartica, "Región de Magallanes y de la Antártica Chilena" }
        };

        public static Dictionary<Region, List<string>> ComunasByRegion = new()
        {
            {
                Region.AricaYParinacota,
                new List<string> { "Arica", "Camarones", "Putre", "General Lagos" }
            },
            {
                Region.Tarapaca,
                new List<string> { "Iquique", "Alto Hospicio", "Pozo Almonte", "Camiña", "Colchane", "Huara", "Pica" }
            },
            {
                Region.Antofagasta,
                new List<string> { "Antofagasta", "Mejillones", "Sierra Gorda", "Taltal", "Calama", "Ollagüe", "San Pedro de Atacama", "Tocopilla", "María Elena" }
            },
            {
                Region.Atacama,
                new List<string> { "Copiapó", "Caldera", "Tierra Amarilla", "Chañaral", "Diego de Almagro", "Vallenar", "Freirina", "Huasco", "Alto del Carmen" }
            },
            {
                Region.Coquimbo,
                new List<string> { "La Serena", "Coquimbo", "Andacollo", "La Higuera", "Vicuña", "Paihuano", "Ovalle", "Monte Patria", "Punitaqui", "Combarbalá", "Illapel", "Salamanca", "Los Vilos", "Canela" }
            },
            {
                Region.Valparaiso,
                new List<string> { "Valparaíso", "Viña del Mar", "Concón", "Quintero", "Puchuncaví", "Casablanca", "Juan Fernández", "San Antonio", "Cartagena", "El Tabo", "El Quisco", "Algarrobo", "Santo Domingo", "Quillota", "La Calera", "Hijuelas", "La Cruz", "Nogales", "San Felipe", "Los Andes", "Calle Larga", "Rinconada", "San Esteban", "Limache", "Olmué", "Villa Alemana", "Petorca", "Cabildo", "Zapallar", "Papudo", "Santa María", "Panquehue", "Llaillay", "Catemu" }
            },
            {
                Region.RegionMetropolitana,
                new List<string> { "Santiago", "Providencia", "Las Condes", "Vitacura", "Lo Barnechea", "Ñuñoa", "La Reina", "Macul", "Peñalolén", "La Florida", "Puente Alto", "San Bernardo", "La Cisterna", "El Bosque", "San Miguel", "San Joaquín", "Pedro Aguirre Cerda", "Lo Espejo", "Estación Central", "Cerrillos", "Maipú", "Pudahuel", "Quilicura", "Renca", "Conchalí", "Independencia", "Recoleta", "Huechuraba", "Colina", "Lampa", "Tiltil", "Pirque", "San José de Maipo", "Buin", "Paine", "Calera de Tango", "Peñaflor", "Talagante", "El Monte", "Isla de Maipo", "Padre Hurtado", "Curacaví", "María Pinto", "Melipilla", "Alhué", "San Pedro" }
            },
            {
                Region.LiberatorOHiggins,
                new List<string> { "Rancagua", "Machalí", "Graneros", "San Francisco de Mostazal", "Doñihue", "Requínoa", "Coltauco", "Codegua", "Olivar", "Rengo", "Malloa", "Quinta de Tilcoco", "San Vicente", "Pichidegua", "Peumo", "Las Cabras", "Pichilemu", "Navidad", "La Estrella", "Marchigüe", "Litueche", "Peralillo", "Chépica", "Santa Cruz", "Palmilla", "Placilla", "Nancagua" }
            },
            {
                Region.Maule,
                new List<string> { "Talca", "Curicó", "Linares", "Cauquenes", "Constitución", "Maule", "San Clemente", "Pelarco", "Río Claro", "San Rafael", "Teno", "Romeral", "Molina", "Sagrada Familia", "Hualañé", "Licantén", "Vichuquén", "Colbún", "Longaví", "Parral", "Retiro", "Villa Alegre", "Yerbas Buenas", "Chanco", "Pelluhue" }
            },
            {
                Region.Nuble,
                new List<string> { "Chillán", "Chillán Viejo", "Bulnes", "San Carlos", "San Nicolás", "Yungay", "Quirihue", "Cobquecura", "Ninhue", "Portezuelo", "Ránquil", "Coihueco", "Pinto", "El Carmen", "Pemuco", "San Ignacio", "Trehuaco", "Quillón" }
            },
            {
                Region.Biobio,
                new List<string> { "Concepción", "Talcahuano", "Hualpén", "San Pedro de la Paz", "Chiguayante", "Penco", "Tomé", "Coronel", "Lota", "Florida", "Hualqui", "Santa Juana", "Lebu", "Arauco", "Cañete", "Los Álamos", "Tirúa", "Los Ángeles", "Nacimiento", "Negrete", "Mulchén", "Quilaco", "Quilleco", "Santa Bárbara", "Yumbel", "Cabrero", "Antuco", "Alto Biobío" }
            },
            {
                Region.LaAraucania,
                new List<string> { "Temuco", "Padre Las Casas", "Angol", "Villarrica", "Pucón", "Freire", "Lautaro", "Nueva Imperial", "Carahue", "Saavedra", "Curacautín", "Lonquimay", "Victoria", "Collipulli", "Ercilla", "Traiguén", "Lumaco", "Melipeuco", "Cunco", "Teodoro Schmidt", "Gorbea", "Toltén", "Perquenco", "Cholchol", "Renaico", "Purén" }
            },
            {
                Region.LosRios,
                new List<string> { "Valdivia", "Corral", "Lanco", "Los Lagos", "Máfil", "Mariquina", "Paillaco", "Panguipulli", "La Unión", "Río Bueno", "Lago Ranco", "Futrono" }
            },
            {
                Region.LosLagos,
                new List<string> { "Puerto Montt", "Puerto Varas", "Osorno", "Castro", "Ancud", "Quellón", "Calbuco", "Maullín", "Frutillar", "Llanquihue", "Fresia", "Purranque", "Río Negro", "San Juan de la Costa", "Chaitén", "Futaleufú", "Palena", "Hualaihué", "Queilén", "Quinchao", "Curaco de Vélez", "Dalcahue", "Puqueldón" }
            },
            {
                Region.Aysen,
                new List<string> { "Coyhaique", "Aysén", "Cisnes", "Guaitecas", "Chile Chico", "Río Ibáñez", "Cochrane", "O'Higgins", "Tortel" }
            },
            {
                Region.MagallanesYAntartica,
                new List<string> { "Punta Arenas", "Puerto Natales", "Porvenir", "Primavera", "Timaukel", "Laguna Blanca", "San Gregorio", "Río Verde", "Cabo de Hornos", "Antártica" }
            }
        };
    }
}