package services;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.MalformedURLException;

import entities.Vehicle;
import httprequest.HttpRequest;
import services.status.Doors;
import services.status.VehicleDetails;
import services.status.Windows;


public class VehicleManager {

    public static Vehicle currentVehicle;
    public static JSONObject vehiclelist;

    public VehicleManager() throws MalformedURLException {
    }


    public static void startVehicleManager() throws IOException, InterruptedException, JSONException {
        //TODO skapa lösning för flera VIN's
        setVehicleVIN();
        updateVehicle();

    }
    public static void updateVehicle() throws IOException, InterruptedException, JSONException {

        currentVehicle.setDoors(Doors.getDoorStatus());
        currentVehicle.setWindows(Windows.getWindowStatus());
        currentVehicle.setVehicleDetails(VehicleDetails.getVehicleDetails());

        // Todo fixa images
//        currentVehicle.saveImage(currentVehicle.getVehicleDetails()
//                .getJSONObject("data")
//                .getJSONObject("images")
//                .getString("exteriorDefaultUrl"),"../res/drawable/car_background_exterior.jpg");
//        currentVehicle.saveImage(currentVehicle.getVehicleDetails()
//                .getJSONObject("data")
//                .getJSONObject("images")
//                .getString("interiorDefaultUrl"),"../res/drawable/car_background_interior.jpg");
    }


    public static void setVehicleVIN() throws IOException, InterruptedException, JSONException {
        vehiclelist = new JSONObject(HttpRequest.createGetRequest("","vehiclelist"));
        //fixa konto med flera bilar todo
        currentVehicle = new Vehicle(vehiclelist.getJSONArray("data").getJSONObject(0).getString("vin"));
        EndPoint.VIN = currentVehicle.getVIN();
    }

}
