import java.util.*;

public class learn{

   	public void areEqual(String temp1,String temp2){
   		System.out.println("areEqual");
   	} 

   	public void areEdit(String temp1,String temp2){
   		System.out.println("areEdit");
   		int[] arry1 = new int[128];
   		for(int i=0;i<128;i++){
   			arry1[i]=0;
   		}
   		for(int i=0;i<temp1.length();i++){
   			int c = (int)temp1.charAt(i);
            arry1[c]++;
   		}   		
   		for(int i=0;i<temp2.length();i++){
   			int c = (int)temp2.charAt(i);
            arry1[c]--;
   		}
   		for(int i=0;i<128;i++){
   			if(arry1[i]){
   			 	System.out.println("Not Valid");	
   			}else{
   				System.out.println("Valid");
   			}
   		}	
   	}

	public static void main(String args[]){
     System.out.println("Hello World!");
	 String temp1 = "plae";
	 String temp2 = "pla";
	 learn obj= new learn();

	 if(temp1.length() == temp2.length())
	   obj.areEqual(temp1,temp2);
	 else if(temp1.length()-1 == temp2.length())
	   obj.areEdit(temp1,temp2);
	}


}