#interval for min and max 
interval = {'minPre':0.3,'midPre':1.8,'maxPre':0.3,'minFlow':2.43, 'midFlow':3.18,'maxFlow':8.12}
#inputs flowrate and netHead
input=[2.5,1.5]
APressure=2
density=1
Pv=3
velocity=0
def getInterval(id):
    if(id=="maxFlowId"):
        return interval['maxFlow']
    if(id=="midFlowId"):
        return interval['midFlow']
    if(id=="minFlowId"):
        return interval['minFlow']
    if(id=="maxPreId"):
        return interval['maxPre']
    if(id=="midPreId"):
        return interval['midPre']
    if(id=="minPreId"):
        return interval['minPre']
def getInput():
    result="{"+str(input[0])+","+str(input[1])+"}"
    return result
def calculateCavityIndex(inputP):
    pressureGage=inputp
    Po=APressure+pressureGage
    Index=Po-Pv/(density)
    return Index
def calculateFlowIndex(flowRate):
    flow=flowRate*Pv
    return flow
#a=calculateCavityIndex("5.2,2.56");
#print(a);