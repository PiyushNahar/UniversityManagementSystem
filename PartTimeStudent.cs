class PartTimeStudent : Student
{

    public override async Task<int> CalculateFeesAsync(int id)
    {
        //await base.CalculateFeesAsync(id);
        Console.WriteLine("As the student is Part time Student the fees is Rs. 4000");
        return 4000;
    }

    public async Task PayFees(int id)
    {
        int amount = await this.CalculateFeesAsync(id);
        await base.PayFees(amount,id);
    }
}
