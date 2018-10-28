using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public static class PopGen {

    private static string[] allNouns;
    private static string[] allAdjectives;
    private static SimulationManager simManager;

    static PopGen(){
        TextAsset nouns = (TextAsset)Resources.Load("nouns", typeof(TextAsset));
        TextAsset adjectives = (TextAsset)Resources.Load("adjectives", typeof(TextAsset));
        allNouns = nouns.text.Split('\n');
        allAdjectives = adjectives.text.Split('\n');
        simManager = SimulationManager.Instance;
    }

    public static string MakeId(long genIter, int orgIter){        
        return genIter.ToString().PadLeft(5,'0') + "-" + orgIter.ToString().PadLeft(5,'0');
    }

    public static Dictionary<string,string[]> MakeOrganismDNA(){
        Dictionary<string, string[]> dna = new Dictionary<string, string[]>();
        dna.Add("color", MakeColorAllele());
        return dna;
    }

    private static string[] MakeColorAllele(){
        return new string[] {
            DecimalToBinary(UnityEngine.Random.Range(0,255)),
            DecimalToBinary(UnityEngine.Random.Range(0,255)),
            DecimalToBinary(UnityEngine.Random.Range(0,255))
        };
    }

    public static string MakeOrganismName(){
        return allAdjectives[UnityEngine.Random.Range(0, allAdjectives.Length)] +
        " " + allNouns[UnityEngine.Random.Range(0, allNouns.Length)];
    }

    public static string DecimalToBinary(int dec){
        return Convert.ToString(dec, 2).PadLeft(8, '0');
    }

    public static int BinaryToDecimal(string binary){
        return Convert.ToInt32(binary, 2);
    }

    public static Color ColorAlleleToColor(string[] colorAllele){
        Color color = new Color();
        color.r = BinaryToDecimal(colorAllele[0])/255f;
        color.g = BinaryToDecimal(colorAllele[1])/255f;
        color.b = BinaryToDecimal(colorAllele[2])/255f;
        color.a = 255f;
        return color;
    }

    public static string AlleleToString(string[] allele){
        string s = "";
        for(int i = 0; i < allele.Length; i++){
            for(int j = 0; j < allele[i].Length; j++){
                s += allele[i][j];
            }
        }
        return s;
    }

    public static int EvaluateFitness(string alleleName, string[] allele){
        switch(alleleName){
            case "color":   return EvaluateColorAllele(allele);
            default:        return 0;                     
        }
    }

    private static int EvaluateColorAllele(string[] allele){
        int score = 0;
        string[] idealAllele = simManager.idealColorAllele;
        for(int i = 0 ; i < allele.Length; i++){
            if(idealAllele[i] != allele[i]){
                score += Math.Abs(Convert.ToInt32(idealAllele[i], 2) - Convert.ToInt32(allele[i], 2));
            }
        }
        return score;
    }

    public static List<GameObject> SetMatingChances(List<GameObject> organisms, int genSize){
        int fifthPop = (int)(genSize * 0.20f);
        int[] fitnessDistrib = new int[]{
            fifthPop,
            genSize-(fifthPop*3),
            genSize-(fifthPop*2),
            genSize-(fifthPop),
            genSize
        };
        for(int i = 0; i < organisms.Count; i++){
            for(int j = 0; j < fitnessDistrib.Length; j++){
                if(i < fitnessDistrib[j]){
                    organisms[i].GetComponent<Organism>().MatingChance = simManager.fitnessWeights[j];
                    break;
                }
            }
        }
        //Set percent of top organisms that can asexually reproduce
        int topMarkers = (int)(genSize * simManager.asexTopOrganisms)-1;
        for(int x = genSize; x > genSize - topMarkers; x++){
            organisms[x].GetComponent<Organism>().HasTopMarker = true;
        }
        return organisms;
    }

    public static List<GameObject> SortByFitness(List<GameObject> organisms){
        IEnumerable<GameObject> query = organisms.OrderByDescending(organism => organism.GetComponent<Organism>().Fitness);
        int i = 0; //would do normal for loop, but can't figure out index iteration with this IEnumerable nonsense
        foreach(GameObject org in query){
            organisms[i] = org;
            i++;
        }
        return organisms;
    }

    public static bool CheckForIdeal(List<GameObject> organisms){
        for(int i = 0; i < organisms.Count; i++){
            if(organisms[i].GetComponent<Organism>().Fitness <= simManager.idealError){
                return true;
            }
        }
        return false;
    }

    public static List<GameObject> GenerateMatingPool(List<GameObject> organisms){
        int genSize = organisms.Count;
        List<GameObject> pool = new List<GameObject>();
        for(int i = 0; i < genSize; i++){
            for(int j = 0; j < organisms[i].GetComponent<Organism>().MatingChance; j++){
                pool.Add(organisms[i]);
            }
        }
        return pool;
    }

    public static GameObject Reproduce(GameObject parent1, GameObject parent2, long genIter, int orgIter){
        Organism org1 = parent1.GetComponent<Organism>();
        Organism org2 = parent2.GetComponent<Organism>();
        GameObject tmp = new GameObject();
        tmp.AddComponent<Organism>();
        Organism offspring = tmp.GetComponent<Organism>();
        Dictionary<string, string[]> dna = new Dictionary<string, string[]>();
        string[] parents = new string[2]{org1.Id, org2.Id};
        if(UnityEngine.Random.Range(0f,1f) < simManager.asexReproChance || org1.Id == org2.Id){
            if(UnityEngine.Random.Range(0f,1f) < 0.50f){
                dna = org1.DNA;
                parents = new string[]{org1.Id, org1.Id};
            } else{
                dna = org2.DNA;
                parents = new string[]{org2.Id, org2.Id};
            }
        } else{
            dna = Crossover(org1.DNA, org2.DNA);
        }

        offspring.name = MakeOrganismName();
        offspring.Generation = genIter;
        offspring.Id = MakeId(genIter, orgIter);
        offspring.DNA = Mutate(dna);
        offspring.Parents = parents;
        offspring.Fitness = EvaluateFitness("color", dna["color"]);
        return tmp;
    }

    private static Dictionary<string, string[]> Crossover(Dictionary<string, string[]> parent1, Dictionary<string, string[]> parent2){
        Dictionary<string, string[]> offspringDNA = new Dictionary<string, string[]>();

        foreach(KeyValuePair<string, string[]> entry in parent1){
            offspringDNA.Add(entry.Key, new string[entry.Value.Length]);
            int crossoverMethod = UnityEngine.Random.Range(0,4);
            string crossPoint = "";

            for(int i = 0; i < entry.Value.Length; i++){
                switch(crossoverMethod){
                    case 0:
                        crossPoint = (UnityEngine.Random.Range(0f,1f) < 0.50f) ? parent1[entry.Key][i] : parent2[entry.Key][i];
                        break;
                    case 1:
                        crossPoint = ParityCrossover(parent1[entry.Key][i], parent2[entry.Key][i]);
                        break;
                    case 2:
                        crossPoint = ArithmeticCrossover(parent1[entry.Key][i], parent2[entry.Key][i]);
                        break;
                    default:
                        crossPoint = UniformCrossover(parent1[entry.Key][i], parent2[entry.Key][i]);
                        break;
                }
                offspringDNA[entry.Key][i] = crossPoint;
            }
        }
        return offspringDNA;
    }

    private static string ParityCrossover(string strand1, string strand2){
        string offspringStrand = "";
        for(int i = 0; i < strand1.Length; i++){
            offspringStrand += (i % 2 == 0) ? strand1[i] : strand2[i];
        }
        return offspringStrand;
    }

    private static string ArithmeticCrossover(string strand1, string strand2){
        string offspringStrand = "";
        for(int i = 0; i < strand1.Length; i++){
            if(UnityEngine.Random.Range(0f,1f) < 0.50f){
                offspringStrand += (strand1[i] == '1' && strand2[i] == '1') ? "1" : "0";
            } else{
                offspringStrand += (strand1[i] == '1' || strand2[i] == '1') ? "1" : "0";
            }
        }
        return offspringStrand;
    }

    private static string UniformCrossover(string strand1, string strand2){
        string offspringStrand = "";
        for(int i = 0; i < strand1.Length; i++){
            offspringStrand += (UnityEngine.Random.Range(0f,1f) < 0.50f) ? strand1[i] : strand2[i];
        }
        return offspringStrand;
    }

    private static Dictionary<string,string[]> Mutate(Dictionary<string,string[]> dna){
        foreach(KeyValuePair<string, string[]> entry in dna){
            string[] strands = entry.Value;
            for(int i = 0; i < strands.Length; i++){
                string newStrand = "";
                for(int j = 0; j < strands[i].Length; j++){
                    if(UnityEngine.Random.Range(0f,1f) < simManager.mutationChance){
                        newStrand += (strands[i][j] == '1') ? '0' : '1';
                    } else{
                        newStrand += strands[i][j];
                    }
                }
                dna[entry.Key][i] = newStrand;
            }
        }
        return dna;
    }
}