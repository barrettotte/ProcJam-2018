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

    public static string MakeId(int genIter, int orgIter){        
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

    public static List<Organism> SetMatingChances(List<Organism> organisms, int genSize){
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
                    organisms[i].MatingChance = simManager.fitnessWeights[j];
                    break;
                }
            }
        }
        return organisms;
    }

    public static List<Organism> SortByFitness(List<Organism> organisms){
        IEnumerable<Organism> query = organisms.OrderByDescending(organism => organism.Fitness);
        int i = 0;
        foreach(Organism org in query){
            organisms[i] = org;
            i++;
        }
        return organisms;
    }

    public static bool CheckForIdeal(List<Organism> organisms){
        for(int i = 0; i < organisms.Count; i++){
            if(organisms[i].Fitness <= simManager.idealError){
                return true;
            }
        }
        return false;
    }

    public static List<Organism> GenerateMatingPool(List<Organism> organisms){
        int genSize = organisms.Count;
        List<Organism> pool = new List<Organism>();
        for(int i = 0; i < genSize; i++){
            for(int j = 0; j < organisms[i].MatingChance; j++){
                pool.Add(organisms[i]);
            }
        }
        return pool;
    }

}