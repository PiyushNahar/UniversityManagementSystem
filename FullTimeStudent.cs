class FullTimeStudent : Student
{

    public override async Task<int> CalculateFeesAsync(int id)
    {
        //await base.CalculateFeesAsync();
        Console.WriteLine("As the student is full time Student the fees is Rs. 10000");
        return 10000;
    }

    public async Task PayFees(int id)
    {
        int amount = await this.CalculateFeesAsync(id);
        await base.PayFees(amount,id);
    }
}
